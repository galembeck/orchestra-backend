using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBCompanyInvitation")]
public class CompanyInvitation : BaseEntity, IBaseEntity<CompanyInvitation>
{
    public string CompanyId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RoleKey { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }
    public string? AcceptedByUserId { get; set; }
    public string InvitedBy { get; set; } = string.Empty;

    public CompanyInvitation WithoutRelations(CompanyInvitation entity)
    {
        if (entity == null) return null!;

        var newEntity = new CompanyInvitation
        {
            CompanyId = entity.CompanyId,
            Email = entity.Email,
            RoleKey = entity.RoleKey,
            Token = entity.Token,
            ExpiresAt = entity.ExpiresAt,
            AcceptedAt = entity.AcceptedAt,
            AcceptedByUserId = entity.AcceptedByUserId,
            InvitedBy = entity.InvitedBy,
        };

        newEntity.InitializeInstance(entity);
        return newEntity;
    }
}
