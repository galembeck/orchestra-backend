using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBService")]
public class Service : BaseEntity, IBaseEntity<Service>
{
    public string CompanyId { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;

    // Free-text specialization (e.g. "encanamento", "ar-condicionado").
    public string ServiceType { get; set; } = string.Empty;

    public decimal? Price { get; set; }
    public bool? Budgetable { get; set; }

    // Aggregates kept in sync by review writes.
    public double Rating { get; set; }
    public int ReviewsCount { get; set; }

    public bool IsActive { get; set; } = true;

    public Service WithoutRelations(Service entity)
    {
        if (entity == null)
            return null!;

        var newEntity = new Service
        {
            CompanyId = entity.CompanyId,
            CategoryId = entity.CategoryId,
            ServiceType = entity.ServiceType,
            Price = entity.Price,
            Budgetable = entity.Budgetable,
            Rating = entity.Rating,
            ReviewsCount = entity.ReviewsCount,
            IsActive = entity.IsActive,
        };

        newEntity.InitializeInstance(entity);
        return newEntity;
    }
}
