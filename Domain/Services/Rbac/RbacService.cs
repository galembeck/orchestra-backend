using Domain.Constants;
using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Repository;

namespace Domain.Services;

public class RbacService(
    IPermissionRepository permissionRepository,
    IRoleRepository roleRepository,
    IRolePermissionRepository rolePermissionRepository,
    ICompanyMemberRepository companyMemberRepository) : IRbacService
{
    public async Task<HashSet<string>> GetUserPermissionsForCompanyAsync(string userId, string companyId, CancellationToken cancellationToken = default)
    {
        var memberships = await companyMemberRepository.GetByUserAsync(userId, cancellationToken);
        var roleIds = memberships
            .Where(m => m.CompanyId == companyId)
            .Select(m => m.RoleId)
            .ToList();

        if (roleIds.Count == 0)
            return new HashSet<string>();

        var keys = await rolePermissionRepository.GetPermissionKeysForRolesAsync(roleIds, cancellationToken);
        return new HashSet<string>(keys, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<bool> HasPermissionAsync(User user, string companyId, string permissionKey, CancellationToken cancellationToken = default)
    {
        if (user is null) return false;

        // Platform developer bypasses company checks for platform-only permissions.
        if (user.ProfileType == ProfileType.PLATFORM_DEVELOPER &&
            PermissionKey.PlatformOnly.Contains(permissionKey))
            return true;

        if (string.IsNullOrEmpty(companyId))
            return false;

        var permissions = await GetUserPermissionsForCompanyAsync(user.Id, companyId, cancellationToken);
        return permissions.Contains(permissionKey);
    }

    public async Task SeedSystemRolesAndPermissionsAsync(CancellationToken cancellationToken = default)
    {
        // Permissions
        var existingKeys = (await permissionRepository.GetByKeysAsync(PermissionKey.All, cancellationToken))
            .Select(p => p.Key).ToHashSet();

        foreach (var key in PermissionKey.All.Where(k => !existingKeys.Contains(k)))
        {
            await permissionRepository.InsertAsync(new Permission
            {
                Key = key,
                Description = key,
                Category = key.Split(':')[0],
            });
        }
    }

    public async Task SeedDefaultRolesForCompanyAsync(string companyId, string actorId, CancellationToken cancellationToken = default)
    {
        foreach (var roleKey in SystemRoleKey.All)
        {
            var existing = await roleRepository.GetByCompanyAndKeyAsync(companyId, roleKey, cancellationToken);
            Role role = existing ?? await roleRepository.InsertAsync(new Role
            {
                CompanyId = companyId,
                Key = roleKey,
                Name = roleKey,
                Description = $"Default {roleKey} role",
                IsSystem = true,
            }, actorId);

            var permKeys = SystemRoleKey.DefaultPermissions[roleKey];
            var perms = await permissionRepository.GetByKeysAsync(permKeys, cancellationToken);
            await rolePermissionRepository.ReplaceRolePermissionsAsync(
                role.Id, perms.Select(p => p.Id), actorId, cancellationToken);
        }
    }

    public async Task<Role> GetOrCreateOwnerRoleAsync(string companyId, string actorId, CancellationToken cancellationToken = default)
    {
        var role = await roleRepository.GetByCompanyAndKeyAsync(companyId, SystemRoleKey.Owner, cancellationToken);
        if (role != null) return role;

        await SeedDefaultRolesForCompanyAsync(companyId, actorId, cancellationToken);
        return (await roleRepository.GetByCompanyAndKeyAsync(companyId, SystemRoleKey.Owner, cancellationToken))!;
    }
}
