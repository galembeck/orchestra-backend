using Domain.Data.Entities;

namespace Domain.Services;

public interface IRbacService
{
    Task<HashSet<string>> GetUserPermissionsForCompanyAsync(string userId, string companyId, CancellationToken cancellationToken = default);
    Task<bool> HasPermissionAsync(User user, string companyId, string permissionKey, CancellationToken cancellationToken = default);

    Task SeedSystemRolesAndPermissionsAsync(CancellationToken cancellationToken = default);
    Task SeedDefaultRolesForCompanyAsync(string companyId, string actorId, CancellationToken cancellationToken = default);
    Task<Role> GetOrCreateOwnerRoleAsync(string companyId, string actorId, CancellationToken cancellationToken = default);
}
