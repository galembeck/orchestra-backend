using Domain.Data.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class ServiceCategoryRepository(AppDbContext context) : BaseRepository<ServiceCategory>(context, context.ServiceCategories), IServiceCategoryRepository
{
    public async Task<List<ServiceCategory>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.IsActive && x.DeletedAt == null)
                .OrderBy(x => x.Name)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task<ServiceCategory?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.Slug == slug && x.DeletedAt == null)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }
}
