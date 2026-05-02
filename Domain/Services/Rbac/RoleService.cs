using Domain.Constants;
using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository;

namespace Domain.Services;

public class RoleService(
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository,
    IRolePermissionRepository rolePermissionRepository) : IRoleService
{
    public Task<List<Role>> GetByCompanyAsync(string companyId, CancellationToken cancellationToken = default)
        => roleRepository.GetByCompanyAsync(companyId, cancellationToken);

    public async Task<Role> CreateAsync(string companyId, string name, string? description, IEnumerable<string> permissionKeys, string actorId, CancellationToken cancellationToken = default)
    {
        var key = name.Trim().ToUpperInvariant().Replace(' ', '_');

        if (await roleRepository.GetByCompanyAndKeyAsync(companyId, key, cancellationToken) is not null)
            throw new BusinessException(BusinessErrorMessage.ROLE_ALREADY_EXISTS);

        var role = await roleRepository.InsertAsync(new Role
        {
            CompanyId = companyId,
            Key = key,
            Name = name,
            Description = description,
            IsSystem = false,
        }, actorId);

        await SetPermissionsAsync(role.Id, permissionKeys, actorId, cancellationToken);
        return role;
    }

    public async Task<Role> UpdateAsync(string roleId, string? name, string? description, string actorId, CancellationToken cancellationToken = default)
    {
        var role = await roleRepository.GetAsync(roleId, cancellationToken);
        if (role.IsSystem)
            throw new BusinessException(BusinessErrorMessage.CANNOT_MODIFY_SYSTEM_ROLE);

        return await roleRepository.UpdatePartialAsync(
            new Role { Id = roleId },
            r =>
            {
                if (!string.IsNullOrWhiteSpace(name)) r.Name = name;
                if (description != null) r.Description = description;
            },
            actorId);
    }

    public async Task DeleteAsync(string roleId, string actorId, CancellationToken cancellationToken = default)
    {
        var role = await roleRepository.GetAsync(roleId, cancellationToken);
        if (role.IsSystem)
            throw new BusinessException(BusinessErrorMessage.CANNOT_MODIFY_SYSTEM_ROLE);

        await roleRepository.DeleteAsync(role, actorId);
    }

    public async Task<List<string>> GetPermissionKeysAsync(string roleId, CancellationToken cancellationToken = default)
    {
        return await rolePermissionRepository.GetPermissionKeysForRolesAsync(new[] { roleId }, cancellationToken);
    }

    public async Task SetPermissionsAsync(string roleId, IEnumerable<string> permissionKeys, string actorId, CancellationToken cancellationToken = default)
    {
        var keys = permissionKeys
            .Where(k => !PermissionKey.PlatformOnly.Contains(k))
            .Distinct()
            .ToList();

        var perms = await permissionRepository.GetByKeysAsync(keys, cancellationToken);
        await rolePermissionRepository.ReplaceRolePermissionsAsync(
            roleId, perms.Select(p => p.Id), actorId, cancellationToken);
    }
}
