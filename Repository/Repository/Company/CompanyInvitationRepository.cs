using Domain.Data.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class CompanyInvitationRepository(AppDbContext context)
    : BaseRepository<CompanyInvitation>(context, context.CompanyInvitations), ICompanyInvitationRepository
{
    public async Task<CompanyInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.Token == token && x.DeletedAt == null)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task<List<CompanyInvitation>> GetByCompanyAsync(string companyId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.CompanyId == companyId && x.DeletedAt == null)
                .OrderByDescending(x => x.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }
}
