using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface ICompanyRepository : IRepository<Company>
{
    Task<Company?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);
    Task<List<Company>> GetByOwnerAsync(string ownerUserId, CancellationToken cancellationToken = default);
    Task<List<Company>> GetByApprovalStatusAsync(CompanyApprovalStatus status, CancellationToken cancellationToken = default);
}
