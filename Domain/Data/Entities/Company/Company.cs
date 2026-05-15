using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using Domain.Enumerators;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBCompany")]
public class Company : BaseEntity, IBaseEntity<Company>
{
    public string OwnerUserId { get; set; } = string.Empty;

    public Segment Segment { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string SocialReason { get; set; } = string.Empty;
    public string FantasyName { get; set; } = string.Empty;

    public string Zipcode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string? Complement { get; set; }
    public string Neighborhood { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    public CompanyApprovalStatus ApprovalStatus { get; set; } = CompanyApprovalStatus.PENDING;
    public string? ApprovedBy { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }

    // Onboarding configuration fields
    public TimeOnly? OpeningHour { get; set; }
    public TimeOnly? ClosingHour { get; set; }
    public TeamSize? TeamSize { get; set; }
    public int? ServiceRadius { get; set; }
    public ServiceType? ServiceTypes { get; set; }
    public CompanySchedule? Schedule { get; set; }

    public Company WithoutRelations(Company entity)
    {
        if (entity == null)
            return null!;

        var newEntity = new Company
        {
            OwnerUserId = entity.OwnerUserId,
            Segment = entity.Segment,
            Cnpj = entity.Cnpj,
            SocialReason = entity.SocialReason,
            FantasyName = entity.FantasyName,
            Zipcode = entity.Zipcode,
            Address = entity.Address,
            Number = entity.Number,
            Complement = entity.Complement,
            Neighborhood = entity.Neighborhood,
            City = entity.City,
            State = entity.State,
            ApprovalStatus = entity.ApprovalStatus,
            ApprovedBy = entity.ApprovedBy,
            ApprovedAt = entity.ApprovedAt,
            RejectionReason = entity.RejectionReason,
            OpeningHour = entity.OpeningHour,
            ClosingHour = entity.ClosingHour,
            TeamSize = entity.TeamSize,
            ServiceRadius = entity.ServiceRadius,
            ServiceTypes = entity.ServiceTypes,
            Schedule = entity.Schedule,
        };

        newEntity.InitializeInstance(entity);
        return newEntity;
    }
}
