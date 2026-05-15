using Domain.Data.Entities;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface ICompanyInvitationRepository : IRepository<CompanyInvitation>
{
    Task<CompanyInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<List<CompanyInvitation>> GetByCompanyAsync(string companyId, CancellationToken cancellationToken = default);
}
