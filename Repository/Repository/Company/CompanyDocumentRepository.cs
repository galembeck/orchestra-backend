using Domain.Data.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class CompanyDocumentRepository(AppDbContext context)
    : BaseRepository<CompanyDocument>(context, context.CompanyDocuments), ICompanyDocumentRepository
{
    public async Task<List<CompanyDocument>> GetByCompanyAsync(string companyId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.CompanyId == companyId && x.DeletedAt == null)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }
}
