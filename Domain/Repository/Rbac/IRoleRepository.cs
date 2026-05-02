using Domain.Data.Entities;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface IRoleRepository : IRepository<Role>
{
    Task<List<Role>> GetByCompanyAsync(string companyId, CancellationToken cancellationToken = default);
    Task<Role?> GetByCompanyAndKeyAsync(string companyId, string key, CancellationToken cancellationToken = default);
    Task<Role?> GetGlobalByKeyAsync(string key, CancellationToken cancellationToken = default);
}
