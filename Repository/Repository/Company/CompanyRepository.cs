using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class CompanyRepository(AppDbContext context) : BaseRepository<Company>(context, context.Companies), ICompanyRepository
{
    public async Task<Company?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.Cnpj == cnpj && x.DeletedAt == null)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task<List<Company>> GetByOwnerAsync(string ownerUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.OwnerUserId == ownerUserId && x.DeletedAt == null)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task<List<Company>> GetByApprovalStatusAsync(CompanyApprovalStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.ApprovalStatus == status && x.DeletedAt == null)
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
