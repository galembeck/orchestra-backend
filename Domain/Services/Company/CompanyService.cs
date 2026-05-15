using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository;
using Domain.Repository.User;
using Domain.Utils;
using Hangfire;
using Microsoft.AspNetCore.Http;
using System.Transactions;

namespace Domain.Services;

public class CompanyService(
    ICompanyRepository companyRepository,
    ICompanyDocumentRepository companyDocumentRepository,
    ICompanyMemberRepository companyMemberRepository,
    ICompanyInvitationRepository companyInvitationRepository,
    IRoleRepository roleRepository,
    IUserRepository userRepository,
    IUserSecurityInfoRepository userSecurityInfoRepository,
    IRbacService rbacService,
    IFileStorageService fileStorageService,
    IBackgroundJobClient backgroundJobClient) : ICompanyService
{
    private static readonly TimeSpan InvitationLifetime = TimeSpan.FromDays(7);

    private static readonly CompanyDocumentType[] RequiredDocumentTypes =
    {
        CompanyDocumentType.CNPJ_DOCUMENT,
        CompanyDocumentType.ADDRESS_PROOF,
        CompanyDocumentType.OWNER_IDENTITY,
    };

    public async Task<Company> RegisterAsync(
        User owner,
        Company company,
        UserSecurityInfo securityInfo,
        IDictionary<CompanyDocumentType, IFormFile> documents,
        CancellationToken cancellationToken = default)
    {
        if (!owner.AcceptedTerms)
            throw new BusinessException(BusinessErrorMessage.TERMS_MUST_BE_ACCEPTED);

        var missing = RequiredDocumentTypes.Where(t => !documents.ContainsKey(t) || documents[t] is null).ToList();
        if (missing.Count > 0)
            throw new BusinessException(BusinessErrorMessage.MISSING_REQUIRED_DOCUMENTS);

        if (await userRepository.GetByEmailAsync(owner.Email, cancellationToken) is not null)
            throw new BusinessException(BusinessErrorMessage.EMAIL_ALREADY_USED);

        if (!string.IsNullOrWhiteSpace(owner.Document) &&
            await userRepository.GetByDocumentAsync(owner.Document, cancellationToken) is not null)
            throw new BusinessException(BusinessErrorMessage.DOCUMENT_ALREADY_USED);

        if (await companyRepository.GetByCnpjAsync(company.Cnpj, cancellationToken) is not null)
            throw new BusinessException(BusinessErrorMessage.CNPJ_ALREADY_USED);

        owner.AccountType = AccountType.COMPANY;
        owner.ProfileType = ProfileType.CLIENT;
        owner.Password = StringUtil.SHA512(owner.Password);
        owner.AcceptedTermsAt = DateTimeOffset.UtcNow;
        owner.LastAccessAt = DateTimeOffset.UtcNow;

        User savedOwner;
        Company savedCompany;
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            savedOwner = await userRepository.InsertAsync(owner.WithoutRelations(owner));

            if (securityInfo is not null)
            {
                securityInfo.UserId = savedOwner.Id;
                securityInfo.Moment = SecurityInfoMoment.REGISTER;
                await userSecurityInfoRepository.InsertAsync(securityInfo.WithoutRelations(securityInfo));
            }

            company.OwnerUserId = savedOwner.Id;
            company.ApprovalStatus = CompanyApprovalStatus.PENDING;
            savedCompany = await companyRepository.InsertAsync(company.WithoutRelations(company), savedOwner.Id);

            await rbacService.SeedDefaultRolesForCompanyAsync(savedCompany.Id, savedOwner.Id, cancellationToken);
            var ownerRole = await rbacService.GetOrCreateOwnerRoleAsync(savedCompany.Id, savedOwner.Id, cancellationToken);

            await companyMemberRepository.InsertAsync(new CompanyMember
            {
                CompanyId = savedCompany.Id,
                UserId = savedOwner.Id,
                RoleId = ownerRole.Id,
                IsOwner = true,
            }, savedOwner.Id);

            foreach (var (type, file) in documents)
            {
                using var stream = file.OpenReadStream();
                var path = await fileStorageService.UploadFileAsync(stream, file.FileName, $"companies/{savedCompany.Id}");
                var url = fileStorageService.GetFileUrl(path);

                await companyDocumentRepository.InsertAsync(new CompanyDocument
                {
                    CompanyId = savedCompany.Id,
                    Type = type,
                    FileName = file.FileName,
                    FilePath = path,
                    FileUrl = url,
                }, savedOwner.Id);
            }

            scope.Complete();
        }

        var ownerName = savedOwner.Name;
        var ownerEmail = savedOwner.Email;
        var fantasyName = savedCompany.FantasyName;
        backgroundJobClient.Enqueue<IEmailService>(s =>
            s.SendCompanyUnderReviewEmailAsync(ownerName, ownerEmail, fantasyName));

        return savedCompany;
    }

    public async Task<Company> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await companyRepository.GetAsync(id, cancellationToken);
        }
        catch (PersistenceException)
        {
            throw new BusinessException(BusinessErrorMessage.COMPANY_NOT_FOUND);
        }
    }

    public async Task<List<Company>> GetForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var memberships = await companyMemberRepository.GetByUserAsync(userId, cancellationToken);
        var ids = memberships.Select(m => m.CompanyId).Distinct().ToList();
        if (ids.Count == 0) return new List<Company>();
        var companies = await companyRepository.GetByIdListAsync(ids, cancellationToken);
        return companies.ToList();
    }

    public Task<List<Company>> GetByApprovalStatusAsync(CompanyApprovalStatus status, CancellationToken cancellationToken = default)
        => companyRepository.GetByApprovalStatusAsync(status, cancellationToken);

    public Task<List<CompanyDocument>> GetDocumentsAsync(string companyId, CancellationToken cancellationToken = default)
        => companyDocumentRepository.GetByCompanyAsync(companyId, cancellationToken);

    public async Task<Company> ApproveAsync(string companyId, string platformUserId, CancellationToken cancellationToken = default)
    {
        var company = await GetByIdAsync(companyId, cancellationToken);
        if (company.ApprovalStatus != CompanyApprovalStatus.PENDING)
            throw new BusinessException(BusinessErrorMessage.COMPANY_ALREADY_REVIEWED);

        var updated = await companyRepository.UpdatePartialAsync(
            new Company { Id = companyId },
            c =>
            {
                c.ApprovalStatus = CompanyApprovalStatus.APPROVED;
                c.ApprovedBy = platformUserId;
                c.ApprovedAt = DateTimeOffset.UtcNow;
                c.RejectionReason = null;
            },
            platformUserId);

        var owner = await userRepository.GetUserAsync(company.OwnerUserId, cancellationToken);
        backgroundJobClient.Enqueue<IEmailService>(s =>
            s.SendCompanyApprovedEmailAsync(owner.Name, owner.Email, company.FantasyName));

        return updated;
    }

    public async Task<Company> RejectAsync(string companyId, string platformUserId, string reason, CancellationToken cancellationToken = default)
    {
        var company = await GetByIdAsync(companyId, cancellationToken);
        if (company.ApprovalStatus != CompanyApprovalStatus.PENDING)
            throw new BusinessException(BusinessErrorMessage.COMPANY_ALREADY_REVIEWED);

        return await companyRepository.UpdatePartialAsync(
            new Company { Id = companyId },
            c =>
            {
                c.ApprovalStatus = CompanyApprovalStatus.REJECTED;
                c.ApprovedBy = platformUserId;
                c.ApprovedAt = DateTimeOffset.UtcNow;
                c.RejectionReason = reason;
            },
            platformUserId);
    }

    public async Task<Company> UpdateConfigurationAsync(string companyId, string actorId, Company changes, CancellationToken cancellationToken = default)
    {
        return await companyRepository.UpdatePartialAsync(
            new Company { Id = companyId },
            c =>
            {
                if (changes.OpeningHour.HasValue) c.OpeningHour = changes.OpeningHour;
                if (changes.ClosingHour.HasValue) c.ClosingHour = changes.ClosingHour;
                if (changes.TeamSize.HasValue) c.TeamSize = changes.TeamSize;
                if (changes.ServiceRadius.HasValue) c.ServiceRadius = changes.ServiceRadius;
                if (changes.ServiceTypes.HasValue) c.ServiceTypes = changes.ServiceTypes;
                if (changes.Schedule.HasValue) c.Schedule = changes.Schedule;
            },
            actorId);
    }

    public async Task<Company> UpdateAsync(string companyId, string actorId, Company changes, CancellationToken cancellationToken = default)
    {
        return await companyRepository.UpdatePartialAsync(
            new Company { Id = companyId },
            c =>
            {
                if (!string.IsNullOrWhiteSpace(changes.SocialReason)) c.SocialReason = changes.SocialReason;
                if (!string.IsNullOrWhiteSpace(changes.FantasyName)) c.FantasyName = changes.FantasyName;
                if (!string.IsNullOrWhiteSpace(changes.Zipcode)) c.Zipcode = changes.Zipcode;
                if (!string.IsNullOrWhiteSpace(changes.Address)) c.Address = changes.Address;
                if (!string.IsNullOrWhiteSpace(changes.Number)) c.Number = changes.Number;
                if (changes.Complement != null) c.Complement = changes.Complement;
                if (!string.IsNullOrWhiteSpace(changes.Neighborhood)) c.Neighborhood = changes.Neighborhood;
                if (!string.IsNullOrWhiteSpace(changes.City)) c.City = changes.City;
                if (!string.IsNullOrWhiteSpace(changes.State)) c.State = changes.State;
            },
            actorId);
    }

    public async Task<CompanyMember> InviteMemberAsync(string companyId, string userEmail, string roleId, string actorId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(userEmail, cancellationToken)
            ?? throw new BusinessException(BusinessErrorMessage.USER_NOT_FOUND);

        if (await companyMemberRepository.GetMembershipAsync(companyId, user.Id, cancellationToken) is not null)
            throw new BusinessException(BusinessErrorMessage.MEMBER_ALREADY_EXISTS);

        var role = await roleRepository.GetAsync(roleId, cancellationToken);
        if (role.CompanyId != companyId)
            throw new BusinessException(BusinessErrorMessage.ROLE_NOT_FOUND);

        return await companyMemberRepository.InsertAsync(new CompanyMember
        {
            CompanyId = companyId,
            UserId = user.Id,
            RoleId = roleId,
            IsOwner = false,
        }, actorId);
    }

    public async Task RemoveMemberAsync(string companyId, string userId, string actorId, CancellationToken cancellationToken = default)
    {
        var member = await companyMemberRepository.GetMembershipAsync(companyId, userId, cancellationToken)
            ?? throw new BusinessException(BusinessErrorMessage.MEMBER_NOT_FOUND);

        if (member.IsOwner)
            throw new BusinessException(BusinessErrorMessage.CANNOT_REMOVE_OWNER);

        await companyMemberRepository.DeleteAsync(member, actorId);
    }

    public async Task<CompanyMember> AssignRoleAsync(string companyId, string userId, string roleId, string actorId, CancellationToken cancellationToken = default)
    {
        var member = await companyMemberRepository.GetMembershipAsync(companyId, userId, cancellationToken)
            ?? throw new BusinessException(BusinessErrorMessage.MEMBER_NOT_FOUND);

        var role = await roleRepository.GetAsync(roleId, cancellationToken);
        if (role.CompanyId != companyId)
            throw new BusinessException(BusinessErrorMessage.ROLE_NOT_FOUND);

        return await companyMemberRepository.UpdatePartialAsync(
            new CompanyMember { Id = member.Id },
            m => m.RoleId = roleId,
            actorId);
    }

    public Task<List<CompanyMember>> GetMembersAsync(string companyId, CancellationToken cancellationToken = default)
        => companyMemberRepository.GetByCompanyAsync(companyId, cancellationToken);

    public async Task<List<(CompanyMember member, User user, Role? role)>> GetMembersDetailedAsync(
        string companyId, CancellationToken cancellationToken = default)
    {
        var members = await companyMemberRepository.GetByCompanyAsync(companyId, cancellationToken);
        if (members.Count == 0) return new();

        var userIds = members.Select(m => m.UserId).Distinct().ToList();
        var roleIds = members.Select(m => m.RoleId).Distinct().ToList();

        var users = (await userRepository.GetByIdListAsync(userIds, cancellationToken)).ToDictionary(u => u.Id);
        var roles = (await roleRepository.GetByIdListAsync(roleIds, cancellationToken)).ToDictionary(r => r.Id);

        return members
            .Where(m => users.ContainsKey(m.UserId))
            .Select(m =>
            {
                roles.TryGetValue(m.RoleId, out var role);
                return (m, users[m.UserId], role);
            })
            .ToList();
    }

    // ─── Worker onboarding ────────────────────────────────────────────────

    public async Task<CompanyInvitation> CreateWorkerInvitationAsync(
        string companyId, string email, string roleKey, string actorId, CancellationToken cancellationToken = default)
    {
        var company = await GetByIdAsync(companyId, cancellationToken);
        if (company.ApprovalStatus != CompanyApprovalStatus.APPROVED)
            throw new BusinessException(BusinessErrorMessage.COMPANY_NOT_APPROVED);

        var role = await roleRepository.GetByCompanyAndKeyAsync(companyId, roleKey, cancellationToken)
            ?? throw new BusinessException(BusinessErrorMessage.ROLE_NOT_FOUND);

        var token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

        var invitation = new CompanyInvitation
        {
            CompanyId = companyId,
            Email = email.Trim().ToLowerInvariant(),
            RoleKey = role.Key,
            Token = token,
            ExpiresAt = DateTimeOffset.UtcNow.Add(InvitationLifetime),
            InvitedBy = actorId,
        };

        var saved = await companyInvitationRepository.InsertAsync(invitation.WithoutRelations(invitation), actorId);

        return saved;
    }

    public async Task<List<CompanyInvitation>> GetInvitationsAsync(string companyId, CancellationToken cancellationToken = default)
        => await companyInvitationRepository.GetByCompanyAsync(companyId, cancellationToken);

    public async Task<CompanyInvitation> ResendInvitationAsync(string companyId, string invitationId, string actorId, CancellationToken cancellationToken = default)
    {
        CompanyInvitation invite;
        try
        {
            invite = await companyInvitationRepository.GetAsync(invitationId, cancellationToken);
        }
        catch (PersistenceException)
        {
            throw new BusinessException(BusinessErrorMessage.INVITE_NOT_FOUND);
        }

        if (invite.CompanyId != companyId)
            throw new BusinessException(BusinessErrorMessage.INVITE_NOT_FOUND);
        if (invite.AcceptedAt != null)
            throw new BusinessException(BusinessErrorMessage.INVITE_ALREADY_USED);

        var newToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        var newExpiresAt = DateTimeOffset.UtcNow.Add(InvitationLifetime);

        var updated = await companyInvitationRepository.UpdatePartialAsync(
            new CompanyInvitation { Id = invite.Id },
            i =>
            {
                i.Token = newToken;
                i.ExpiresAt = newExpiresAt;
            },
            actorId);

        return updated;
    }

    public async Task RevokeInvitationAsync(string companyId, string invitationId, string actorId, CancellationToken cancellationToken = default)
    {
        CompanyInvitation invite;
        try
        {
            invite = await companyInvitationRepository.GetAsync(invitationId, cancellationToken);
        }
        catch (PersistenceException)
        {
            throw new BusinessException(BusinessErrorMessage.INVITE_NOT_FOUND);
        }

        if (invite.CompanyId != companyId)
            throw new BusinessException(BusinessErrorMessage.INVITE_NOT_FOUND);
        if (invite.AcceptedAt != null)
            throw new BusinessException(BusinessErrorMessage.INVITE_ALREADY_USED);

        await companyInvitationRepository.DeleteAsync(invite, actorId);
    }

    public async Task<CompanyInvitation> GetInvitationByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var invite = await companyInvitationRepository.GetByTokenAsync(token, cancellationToken)
            ?? throw new BusinessException(BusinessErrorMessage.INVITE_NOT_FOUND);

        if (invite.AcceptedAt != null)
            throw new BusinessException(BusinessErrorMessage.INVITE_ALREADY_USED);
        if (invite.ExpiresAt < DateTimeOffset.UtcNow)
            throw new BusinessException(BusinessErrorMessage.INVITE_EXPIRED);

        return invite;
    }

    public async Task<(User user, CompanyMember member)> AcceptWorkerInvitationAsync(
        string token, User newUserData, UserSecurityInfo securityInfo, CancellationToken cancellationToken = default)
    {
        var invite = await GetInvitationByTokenAsync(token, cancellationToken);

        if (!string.Equals(invite.Email, newUserData.Email.Trim().ToLowerInvariant(), StringComparison.Ordinal))
            throw new BusinessException(BusinessErrorMessage.INVITE_EMAIL_MISMATCH);

        var role = await roleRepository.GetByCompanyAndKeyAsync(invite.CompanyId, invite.RoleKey, cancellationToken)
            ?? throw new BusinessException(BusinessErrorMessage.ROLE_NOT_FOUND);

        var (user, member) = await CreateWorkerUserAndMembershipAsync(
            invite.CompanyId, role, newUserData, securityInfo, cancellationToken);

        await companyInvitationRepository.UpdatePartialAsync(
            new CompanyInvitation { Id = invite.Id },
            i =>
            {
                i.AcceptedAt = DateTimeOffset.UtcNow;
                i.AcceptedByUserId = user.Id;
            },
            user.Id);

        return (user, member);
    }

    public async Task<(User user, CompanyMember member)> RegisterWorkerDirectAsync(
        string companyId, string roleKey, User newUserData, UserSecurityInfo securityInfo, CancellationToken cancellationToken = default)
    {
        var company = await GetByIdAsync(companyId, cancellationToken);

        var role = await roleRepository.GetByCompanyAndKeyAsync(companyId, roleKey, cancellationToken)
            ?? throw new BusinessException(BusinessErrorMessage.ROLE_NOT_FOUND);

        return await CreateWorkerUserAndMembershipAsync(companyId, role, newUserData, securityInfo, cancellationToken);
    }

    public async Task<(CompanyMember member, Company company, Role role)?> GetWorkerContextAsync(
        string userId, CancellationToken cancellationToken = default)
    {
        var memberships = await companyMemberRepository.GetByUserAsync(userId, cancellationToken);
        var member = memberships.FirstOrDefault();
        if (member is null) return null;

        var company = await companyRepository.GetAsync(member.CompanyId, cancellationToken);
        var role = await roleRepository.GetAsync(member.RoleId, cancellationToken);
        return (member, company, role);
    }

    private async Task<(User user, CompanyMember member)> CreateWorkerUserAndMembershipAsync(
        string companyId, Role role, User newUserData, UserSecurityInfo securityInfo, CancellationToken cancellationToken)
    {
        if (await userRepository.GetByEmailAsync(newUserData.Email, cancellationToken) is not null)
            throw new BusinessException(BusinessErrorMessage.EMAIL_ALREADY_USED);

        if (!string.IsNullOrWhiteSpace(newUserData.Document) &&
            await userRepository.GetByDocumentAsync(newUserData.Document, cancellationToken) is not null)
            throw new BusinessException(BusinessErrorMessage.DOCUMENT_ALREADY_USED);

        newUserData.AccountType = AccountType.WORKER;
        newUserData.ProfileType = ProfileType.CLIENT;
        newUserData.Email = newUserData.Email.Trim().ToLowerInvariant();
        newUserData.Password = StringUtil.SHA512(newUserData.Password);
        newUserData.AcceptedTerms = true;
        newUserData.AcceptedTermsAt = DateTimeOffset.UtcNow;
        newUserData.LastAccessAt = DateTimeOffset.UtcNow;

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var savedUser = await userRepository.InsertAsync(newUserData.WithoutRelations(newUserData));

        if (securityInfo is not null)
        {
            securityInfo.UserId = savedUser.Id;
            securityInfo.Moment = SecurityInfoMoment.REGISTER;
            await userSecurityInfoRepository.InsertAsync(securityInfo.WithoutRelations(securityInfo));
        }

        var member = await companyMemberRepository.InsertAsync(new CompanyMember
        {
            CompanyId = companyId,
            UserId = savedUser.Id,
            RoleId = role.Id,
            IsOwner = false,
        }, savedUser.Id);

        scope.Complete();

        return (savedUser, member);
    }
}
