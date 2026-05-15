using Domain.Data.Entities;

namespace API.Public.DTOs;

public class CompanyMemberDTO
{
    public string Id { get; set; } = string.Empty;
    public string CompanyId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public bool IsOwner { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public static CompanyMemberDTO ModelToDTO(CompanyMember m) => new()
    {
        Id = m.Id,
        CompanyId = m.CompanyId,
        UserId = m.UserId,
        RoleId = m.RoleId,
        IsOwner = m.IsOwner,
        CreatedAt = m.CreatedAt,
    };

    public static List<CompanyMemberDTO> ModelToDTO(IEnumerable<CompanyMember> list) =>
        list.Select(ModelToDTO).ToList();
}

public class CompanyMemberDetailDTO
{
    public string Id { get; set; } = string.Empty;
    public string CompanyId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public bool IsOwner { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTimeOffset? LastAccessAt { get; set; }

    public string? RoleName { get; set; }
    public string? RoleKey { get; set; }

    public static CompanyMemberDetailDTO ModelToDTO(CompanyMember m, User u, Role? r) => new()
    {
        Id = m.Id,
        CompanyId = m.CompanyId,
        UserId = m.UserId,
        RoleId = m.RoleId,
        IsOwner = m.IsOwner,
        CreatedAt = m.CreatedAt,
        Name = u.Name,
        Email = u.Email,
        AvatarUrl = u.AvatarUrl,
        LastAccessAt = u.LastAccessAt,
        RoleName = r?.Name,
        RoleKey = r?.Key,
    };
}

public class InviteMemberDTO
{
    public string UserEmail { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
}

public class AssignRoleDTO
{
    public string RoleId { get; set; } = string.Empty;
}
