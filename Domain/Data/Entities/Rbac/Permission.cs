using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBPermission")]
public class Permission : BaseEntity, IBaseEntity<Permission>
{
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }

    public Permission WithoutRelations(Permission entity)
    {
        if (entity == null)
            return null!;

        var newEntity = new Permission
        {
            Key = entity.Key,
            Description = entity.Description,
            Category = entity.Category,
        };

        newEntity.InitializeInstance(entity);
        return newEntity;
    }
}
