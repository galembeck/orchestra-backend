using Domain.Data.Entities;

namespace Domain.Services;

public interface IServiceCategoryService
{
    Task<List<ServiceCategory>> ListActiveAsync(CancellationToken cancellationToken = default);
    Task<ServiceCategory> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}
