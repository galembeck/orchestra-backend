using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Utils;

namespace API.Public.DTOs;

public class PublicCompanyDTO
{
    public string Id { get; set; } = string.Empty;
    public string OwnerUserId { get; set; } = string.Empty;
    public Segment Segment { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string SocialReason { get; set; } = string.Empty;
    public string FantasyName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public string Zipcode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string? Complement { get; set; }
    public string Neighborhood { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    public CompanyApprovalStatus ApprovalStatus { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    // Onboarding configuration
    public string? OpeningHour { get; set; }
    public string? ClosingHour { get; set; }
    public TeamSize? TeamSize { get; set; }
    public int? ServiceRadius { get; set; }
    public List<ServiceType>? ServiceTypes { get; set; }
    public CompanySchedule? Schedule { get; set; }

    public static PublicCompanyDTO ModelToDTO(Company c) => new()
    {
        Id = c.Id,
        OwnerUserId = c.OwnerUserId,
        Segment = c.Segment,
        Cnpj = c.Cnpj,
        SocialReason = c.SocialReason,
        FantasyName = c.FantasyName,
        Slug = StringUtil.GenerateSlug(c.FantasyName),
        Zipcode = c.Zipcode,
        Address = c.Address,
        Number = c.Number,
        Complement = c.Complement,
        Neighborhood = c.Neighborhood,
        City = c.City,
        State = c.State,
        ApprovalStatus = c.ApprovalStatus,
        ApprovedBy = c.ApprovedBy,
        ApprovedAt = c.ApprovedAt,
        RejectionReason = c.RejectionReason,
        CreatedAt = c.CreatedAt,
        OpeningHour = c.OpeningHour?.ToString("HH:mm"),
        ClosingHour = c.ClosingHour?.ToString("HH:mm"),
        TeamSize = c.TeamSize,
        ServiceRadius = c.ServiceRadius,
        ServiceTypes = c.ServiceTypes.HasValue
            ? Enum.GetValues<ServiceType>()
                  .Where(f => c.ServiceTypes.Value.HasFlag(f))
                  .ToList()
            : null,
        Schedule = c.Schedule,
    };

    public static List<PublicCompanyDTO> ModelToDTO(IEnumerable<Company> list) =>
        list.Select(ModelToDTO).ToList();
}
