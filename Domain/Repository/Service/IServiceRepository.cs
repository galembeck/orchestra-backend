using Domain.Data.Entities;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface IServiceRepository : IRepository<Service>
{
    Task<List<Service>> GetByCompanyAsync(string companyId, CancellationToken cancellationToken = default);
    Task<List<Service>> GetActiveAsync(CancellationToken cancellationToken = default);
}
