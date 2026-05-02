using Domain.Data.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class RolePermissionRepository(AppDbContext context)
    : BaseRepository<RolePermission>(context, context.RolePermissions), IRolePermissionRepository
{
    public async Task<List<RolePermission>> GetByRoleAsync(string roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.RoleId == roleId && x.DeletedAt == null)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task<List<string>> GetPermissionKeysForRolesAsync(IEnumerable<string> roleIds, CancellationToken cancellationToken = default)
    {
        try
        {
            return await (from rp in _context.RolePermissions
                          join p in _context.Permissions on rp.PermissionId equals p.Id
                          where roleIds.Contains(rp.RoleId)
                                && rp.DeletedAt == null
                                && p.DeletedAt == null
                          select p.Key)
                          .Distinct()
                          .AsNoTracking()
                          .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task ReplaceRolePermissionsAsync(string roleId, IEnumerable<string> permissionIds, string actorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var existing = await _entity
                .Where(x => x.RoleId == roleId && x.DeletedAt == null)
                .ToListAsync(cancellationToken);

            foreach (var item in existing)
            {
                item.DeletedAt = DateTimeOffset.UtcNow;
                item.UpdatedAt = DateTimeOffset.UtcNow;
                item.UpdatedBy = actorId;
            }

            var now = DateTimeOffset.UtcNow;
            var toAdd = permissionIds.Distinct().Select(pid => new RolePermission
            {
                Id = Guid.NewGuid().ToString(),
                RoleId = roleId,
                PermissionId = pid,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = actorId,
                UpdatedBy = actorId,
            }).ToList();

            await _entity.AddRangeAsync(toAdd, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }
}
