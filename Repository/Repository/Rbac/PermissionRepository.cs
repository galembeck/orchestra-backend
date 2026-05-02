using Domain.Data.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class PermissionRepository(AppDbContext context)
    : BaseRepository<Permission>(context, context.Permissions), IPermissionRepository
{
    public async Task<Permission?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.Key == key && x.DeletedAt == null)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task<List<Permission>> GetByKeysAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => keys.Contains(x.Key) && x.DeletedAt == null)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }
}
