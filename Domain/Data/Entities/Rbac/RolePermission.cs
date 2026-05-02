using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBRolePermission")]
public class RolePermission : BaseEntity, IBaseEntity<RolePermission>
{
    public string RoleId { get; set; } = string.Empty;
    public string PermissionId { get; set; } = string.Empty;

    public RolePermission WithoutRelations(RolePermission entity)
    {
        if (entity == null)
            return null!;

        var newEntity = new RolePermission
        {
            RoleId = entity.RoleId,
            PermissionId = entity.PermissionId,
        };

        newEntity.InitializeInstance(entity);
        return newEntity;
    }
}
