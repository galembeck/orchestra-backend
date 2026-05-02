using Domain.Data.Entities;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface ICompanyDocumentRepository : IRepository<CompanyDocument>
{
    Task<List<CompanyDocument>> GetByCompanyAsync(string companyId, CancellationToken cancellationToken = default);
}
