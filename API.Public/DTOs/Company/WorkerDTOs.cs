using Domain.Data.Entities;

namespace API.Public.DTOs;

public class CreateWorkerInvitationDTO
{
    public string Email { get; set; } = string.Empty;
    public string RoleKey { get; set; } = string.Empty;
}

public class CompanyInvitationDTO
{
    public string Id { get; set; } = string.Empty;
    public string CompanyId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RoleKey { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }

    public static CompanyInvitationDTO ModelToDTO(CompanyInvitation i) => new()
    {
        Id = i.Id,
        CompanyId = i.CompanyId,
        Email = i.Email,
        RoleKey = i.RoleKey,
        Token = i.Token,
        ExpiresAt = i.ExpiresAt,
        AcceptedAt = i.AcceptedAt,
    };
}

public class AcceptWorkerInvitationDTO
{
    public string Token { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cellphone { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// Dev/test-only: skip the invitation flow and create a worker directly.
public class RegisterWorkerDirectDTO
{
    public string CompanyId { get; set; } = string.Empty;
    public string RoleKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cellphone { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
