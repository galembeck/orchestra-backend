using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBCompanyMember")]
public class CompanyMember : BaseEntity, IBaseEntity<CompanyMember>
{
    public string CompanyId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public bool IsOwner { get; set; }

    public CompanyMember WithoutRelations(CompanyMember entity)
    {
        if (entity == null)
            return null!;

        var newEntity = new CompanyMember
        {
            CompanyId = entity.CompanyId,
            UserId = entity.UserId,
            RoleId = entity.RoleId,
            IsOwner = entity.IsOwner,
        };

        newEntity.InitializeInstance(entity);
        return newEntity;
    }
}
