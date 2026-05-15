using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBServiceCategory")]
public class ServiceCategory : BaseEntity, IBaseEntity<ServiceCategory>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Icon { get; set; } = "Wrench";
    public bool IsActive { get; set; } = true;

    public ServiceCategory WithoutRelations(ServiceCategory entity)
    {
        if (entity == null)
            return null!;

        var newEntity = new ServiceCategory
        {
            Name = entity.Name,
            Slug = entity.Slug,
            Icon = entity.Icon,
            IsActive = entity.IsActive,
        };

        newEntity.InitializeInstance(entity);
        return newEntity;
    }
}
