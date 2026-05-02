using Domain.Data.Entities;

namespace Domain.Services;

public interface IRoleService
{
    Task<List<Role>> GetByCompanyAsync(string companyId, CancellationToken cancellationToken = default);
    Task<Role> CreateAsync(string companyId, string name, string? description, IEnumerable<string> permissionKeys, string actorId, CancellationToken cancellationToken = default);
    Task<Role> UpdateAsync(string roleId, string? name, string? description, string actorId, CancellationToken cancellationToken = default);
    Task DeleteAsync(string roleId, string actorId, CancellationToken cancellationToken = default);
    Task<List<string>> GetPermissionKeysAsync(string roleId, CancellationToken cancellationToken = default);
    Task SetPermissionsAsync(string roleId, IEnumerable<string> permissionKeys, string actorId, CancellationToken cancellationToken = default);
}
