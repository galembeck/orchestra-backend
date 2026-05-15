using Domain.Data.Entities;

namespace Domain.Services;

public interface IServiceManagementService
{
    Task<Service> CreateAsync(string companyId, string actorId, Service input, CancellationToken cancellationToken = default);
    Task<List<(Service service, ServiceCategory category, Company company)>> ListByCompanyAsync(string companyId, CancellationToken cancellationToken = default);
    Task<List<(Service service, ServiceCategory category, Company company)>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<(Service service, ServiceCategory category, Company company)> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}
