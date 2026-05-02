using Domain.Data.Entities;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface IRolePermissionRepository : IRepository<RolePermission>
{
    Task<List<RolePermission>> GetByRoleAsync(string roleId, CancellationToken cancellationToken = default);
    Task<List<string>> GetPermissionKeysForRolesAsync(IEnumerable<string> roleIds, CancellationToken cancellationToken = default);
    Task ReplaceRolePermissionsAsync(string roleId, IEnumerable<string> permissionIds, string actorId, CancellationToken cancellationToken = default);
}
