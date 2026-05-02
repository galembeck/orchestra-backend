using Domain.Data.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class RoleRepository(AppDbContext context)
    : BaseRepository<Role>(context, context.Roles), IRoleRepository
{
    public async Task<List<Role>> GetByCompanyAsync(string companyId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.CompanyId == companyId && x.DeletedAt == null)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task<Role?> GetByCompanyAndKeyAsync(string companyId, string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.CompanyId == companyId && x.Key == key && x.DeletedAt == null)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task<Role?> GetGlobalByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.CompanyId == null && x.Key == key && x.DeletedAt == null)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }
}
