using Domain.Data.Entities;
using Domain.Enumerators;

namespace API.Public.DTOs;

public class CompanyContextDTO
{
    public string CompanyId { get; set; } = string.Empty;
    public string FantasyName { get; set; } = string.Empty;
    public string SocialReason { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public CompanyApprovalStatus ApprovalStatus { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public bool IsOwner { get; set; }

    public static CompanyContextDTO Build(Company company, bool isOwner) => new()
    {
        CompanyId = company.Id,
        FantasyName = company.FantasyName,
        SocialReason = company.SocialReason,
        Cnpj = company.Cnpj,
        ApprovalStatus = company.ApprovalStatus,
        ApprovedAt = company.ApprovedAt,
        City = company.City,
        State = company.State,
        IsOwner = isOwner,
    };
}
