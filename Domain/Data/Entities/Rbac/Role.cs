using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBRole")]
public class Role : BaseEntity, IBaseEntity<Role>
{
    public string? CompanyId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystem { get; set; }

    public Role WithoutRelations(Role entity)
    {
        if (entity == null)
            return null!;

        var newEntity = new Role
        {
            CompanyId = entity.CompanyId,
            Key = entity.Key,
            Name = entity.Name,
            Description = entity.Description,
            IsSystem = entity.IsSystem,
        };

        newEntity.InitializeInstance(entity);
        return newEntity;
    }
}
