using Domain.Data.Entities;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface IPermissionRepository : IRepository<Permission>
{
    Task<Permission?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<List<Permission>> GetByKeysAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);
}
