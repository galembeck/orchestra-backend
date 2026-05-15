using Domain.Data.Entities;
using Domain.Enumerators;
using Microsoft.AspNetCore.Http;

namespace Domain.Services;

public interface ICompanyService
{
    Task<Company> RegisterAsync(
        User owner,
        Company company,
        UserSecurityInfo securityInfo,
        IDictionary<CompanyDocumentType, IFormFile> documents,
        CancellationToken cancellationToken = default);

    Task<Company> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<Company>> GetForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<Company>> GetByApprovalStatusAsync(CompanyApprovalStatus status, CancellationToken cancellationToken = default);
    Task<List<CompanyDocument>> GetDocumentsAsync(string companyId, CancellationToken cancellationToken = default);

    Task<Company> ApproveAsync(string companyId, string platformUserId, CancellationToken cancellationToken = default);
    Task<Company> RejectAsync(string companyId, string platformUserId, string reason, CancellationToken cancellationToken = default);

    Task<Company> UpdateAsync(string companyId, string actorId, Company changes, CancellationToken cancellationToken = default);
    Task<Company> UpdateConfigurationAsync(string companyId, string actorId, Company changes, CancellationToken cancellationToken = default);

    Task<CompanyMember> InviteMemberAsync(string companyId, string userEmail, string roleId, string actorId, CancellationToken cancellationToken = default);
    Task RemoveMemberAsync(string companyId, string userId, string actorId, CancellationToken cancellationToken = default);
    Task<CompanyMember> AssignRoleAsync(string companyId, string userId, string roleId, string actorId, CancellationToken cancellationToken = default);
    Task<List<CompanyMember>> GetMembersAsync(string companyId, CancellationToken cancellationToken = default);
    Task<List<(CompanyMember member, User user, Role? role)>> GetMembersDetailedAsync(string companyId, CancellationToken cancellationToken = default);

    // Worker invitations + onboarding
    Task<CompanyInvitation> CreateWorkerInvitationAsync(string companyId, string email, string roleKey, string actorId, CancellationToken cancellationToken = default);
    Task<List<CompanyInvitation>> GetInvitationsAsync(string companyId, CancellationToken cancellationToken = default);
    Task<CompanyInvitation> ResendInvitationAsync(string companyId, string invitationId, string actorId, CancellationToken cancellationToken = default);
    Task RevokeInvitationAsync(string companyId, string invitationId, string actorId, CancellationToken cancellationToken = default);
    Task<CompanyInvitation> GetInvitationByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<(User user, CompanyMember member)> AcceptWorkerInvitationAsync(string token, User newUserData, UserSecurityInfo securityInfo, CancellationToken cancellationToken = default);
    Task<(User user, CompanyMember member)> RegisterWorkerDirectAsync(string companyId, string roleKey, User newUserData, UserSecurityInfo securityInfo, CancellationToken cancellationToken = default);
    Task<(CompanyMember member, Company company, Role role)?> GetWorkerContextAsync(string userId, CancellationToken cancellationToken = default);
}
