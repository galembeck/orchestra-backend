using Domain.Data.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class ServiceRepository(AppDbContext context) : BaseRepository<Service>(context, context.Services), IServiceRepository
{
    public async Task<List<Service>> GetByCompanyAsync(string companyId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.CompanyId == companyId && x.DeletedAt == null)
                .OrderByDescending(x => x.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task<List<Service>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.IsActive && x.DeletedAt == null)
                .OrderByDescending(x => x.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }
}
