using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Domain.Services;

public class ServiceManagementService(
    IServiceRepository serviceRepository,
    IServiceCategoryRepository serviceCategoryRepository,
    ICompanyRepository companyRepository) : IServiceManagementService
{
    public async Task<Service> CreateAsync(
        string companyId,
        string actorId,
        Service input,
        CancellationToken cancellationToken = default)
    {
        var company = await companyRepository.GetAsync(companyId, cancellationToken)
            ?? throw new BusinessException(BusinessErrorMessage.COMPANY_NOT_FOUND);

        var category = await serviceCategoryRepository.GetByExpression(
                c => c.Id == input.CategoryId && c.IsActive && c.DeletedAt == null)
            .FirstOrDefaultAsync(cancellationToken);
        if (category is null)
            throw new BusinessException(BusinessErrorMessage.SERVICE_CATEGORY_NOT_FOUND);

        var service = new Service
        {
            CompanyId = company.Id,
            CategoryId = category.Id,
            ServiceType = input.ServiceType?.Trim() ?? string.Empty,
            Price = input.Price,
            Budgetable = input.Budgetable,
            Rating = 0,
            ReviewsCount = 0,
            IsActive = true,
        };

        return await serviceRepository.InsertAsync(service.WithoutRelations(service), actorId);
    }

    public async Task<List<(Service service, ServiceCategory category, Company company)>> ListByCompanyAsync(
        string companyId, CancellationToken cancellationToken = default)
    {
        var services = await serviceRepository.GetByCompanyAsync(companyId, cancellationToken);
        return await JoinAsync(services, cancellationToken);
    }

    public async Task<List<(Service service, ServiceCategory category, Company company)>> ListAllAsync(
        CancellationToken cancellationToken = default)
    {
        var services = await serviceRepository.GetActiveAsync(cancellationToken);
        return await JoinAsync(services, cancellationToken);
    }

    public async Task<(Service service, ServiceCategory category, Company company)> GetByIdAsync(
        string id, CancellationToken cancellationToken = default)
    {
        var service = await serviceRepository.GetByExpression(s => s.Id == id && s.DeletedAt == null)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BusinessException(BusinessErrorMessage.SERVICE_NOT_FOUND);

        var joined = await JoinAsync(new List<Service> { service }, cancellationToken);
        return joined.Single();
    }

    private async Task<List<(Service, ServiceCategory, Company)>> JoinAsync(
        List<Service> services, CancellationToken cancellationToken)
    {
        if (services.Count == 0) return new();

        var companyIds = services.Select(s => s.CompanyId).Distinct().ToList();
        var categoryIds = services.Select(s => s.CategoryId).Distinct().ToList();

        var companies = (await companyRepository.GetByIdListAsync(companyIds, cancellationToken))
            .ToDictionary(c => c.Id);
        var categories = (await serviceCategoryRepository.GetByIdListAsync(categoryIds, cancellationToken))
            .ToDictionary(c => c.Id);

        return services
            .Where(s => companies.ContainsKey(s.CompanyId) && categories.ContainsKey(s.CategoryId))
            .Select(s => (s, categories[s.CategoryId], companies[s.CompanyId]))
            .ToList();
    }
}
