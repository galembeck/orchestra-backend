using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository;
using Domain.Repository.User;
using Domain.Utils;
using Microsoft.AspNetCore.Http;
using System.Transactions;

namespace Domain.Services;

public class CompanyService(
    ICompanyRepository companyRepository,
    ICompanyDocumentRepository companyDocumentRepository,
    ICompanyMemberRepository companyMemberRepository,
    IRoleRepository roleRepository,
    IUserRepository userRepository,
    IUserSecurityInfoRepository userSecurityInfoRepository,
    IRbacService rbacService,
    IFileStorageService fileStorageService) : ICompanyService
{
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

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var savedOwner = await userRepository.InsertAsync(owner.WithoutRelations(owner));

        if (securityInfo is not null)
        {
            securityInfo.UserId = savedOwner.Id;
            securityInfo.Moment = SecurityInfoMoment.REGISTER;
            await userSecurityInfoRepository.InsertAsync(securityInfo.WithoutRelations(securityInfo));
        }

        company.OwnerUserId = savedOwner.Id;
        company.ApprovalStatus = CompanyApprovalStatus.PENDING;
        var savedCompany = await companyRepository.InsertAsync(company.WithoutRelations(company), savedOwner.Id);

        // Default roles + permissions, then attach owner as OWNER
        await rbacService.SeedDefaultRolesForCompanyAsync(savedCompany.Id, savedOwner.Id, cancellationToken);
        var ownerRole = await rbacService.GetOrCreateOwnerRoleAsync(savedCompany.Id, savedOwner.Id, cancellationToken);

        await companyMemberRepository.InsertAsync(new CompanyMember
        {
            CompanyId = savedCompany.Id,
            UserId = savedOwner.Id,
            RoleId = ownerRole.Id,
            IsOwner = true,
        }, savedOwner.Id);

        // Documents
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

        return await companyRepository.UpdatePartialAsync(
            new Company { Id = companyId },
            c =>
            {
                c.ApprovalStatus = CompanyApprovalStatus.APPROVED;
                c.ApprovedBy = platformUserId;
                c.ApprovedAt = DateTimeOffset.UtcNow;
                c.RejectionReason = null;
            },
            platformUserId);
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
}
