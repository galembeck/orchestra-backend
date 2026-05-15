using Domain.Data.Entities;

namespace API.Public.DTOs;

public class WorkerContextDTO
{
    public string CompanyId { get; set; } = string.Empty;
    public string CompanyFantasyName { get; set; } = string.Empty;
    public string CompanySocialReason { get; set; } = string.Empty;
    public string CompanyCnpj { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public string RoleKey { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public bool IsOwner { get; set; }
    public DateTimeOffset MemberSince { get; set; }

    public static WorkerContextDTO Build(CompanyMember member, Company company, Role role) => new()
    {
        CompanyId = company.Id,
        CompanyFantasyName = company.FantasyName,
        CompanySocialReason = company.SocialReason,
        CompanyCnpj = company.Cnpj,
        RoleId = role.Id,
        RoleKey = role.Key,
        RoleName = role.Name,
        IsOwner = member.IsOwner,
        MemberSince = member.CreatedAt,
    };
}
