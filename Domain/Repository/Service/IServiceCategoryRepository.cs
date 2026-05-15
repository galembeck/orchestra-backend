using Domain.Data.Entities;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface IServiceCategoryRepository : IRepository<ServiceCategory>
{
    Task<List<ServiceCategory>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<ServiceCategory?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
}
