using Domain.Data.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class CompanyMemberRepository(AppDbContext context)
    : BaseRepository<CompanyMember>(context, context.CompanyMembers), ICompanyMemberRepository
{
    public async Task<CompanyMember?> GetMembershipAsync(string companyId, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.CompanyId == companyId && x.UserId == userId && x.DeletedAt == null)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task<List<CompanyMember>> GetByCompanyAsync(string companyId, CancellationToken cancellationToken = default)
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

    public async Task<List<CompanyMember>> GetByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.UserId == userId && x.DeletedAt == null)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }
}
