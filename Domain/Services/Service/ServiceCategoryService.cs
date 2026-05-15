using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Domain.Services;

public class ServiceCategoryService(
    IServiceCategoryRepository serviceCategoryRepository) : IServiceCategoryService
{
    public Task<List<ServiceCategory>> ListActiveAsync(CancellationToken cancellationToken = default)
        => serviceCategoryRepository.GetActiveAsync(cancellationToken);

    public async Task<ServiceCategory> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var category = await serviceCategoryRepository.GetByExpression(c => c.Id == id && c.DeletedAt == null)
            .FirstOrDefaultAsync(cancellationToken);
        if (category is null)
            throw new BusinessException(BusinessErrorMessage.SERVICE_CATEGORY_NOT_FOUND);
        return category;
    }
}
