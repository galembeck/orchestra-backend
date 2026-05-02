using Domain.Data.Entities;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface ICompanyMemberRepository : IRepository<CompanyMember>
{
    Task<CompanyMember?> GetMembershipAsync(string companyId, string userId, CancellationToken cancellationToken = default);
    Task<List<CompanyMember>> GetByCompanyAsync(string companyId, CancellationToken cancellationToken = default);
    Task<List<CompanyMember>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
}
