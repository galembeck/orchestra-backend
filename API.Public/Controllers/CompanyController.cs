using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.Filters;
using API.Public.Validators;
using Domain.Constants;
using Domain.Enumerators;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Public.Controllers;

[ApiController]
[Route("[controller]")]
public class CompanyController(
    ICompanyService companyService,
    IHttpContextAccessor httpContextAccessor) : _BaseController(httpContextAccessor)
{
    private readonly ICompanyService _companyService = companyService;

    [HttpPost("register")]
    [AllowAnonymous]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Register([FromForm] RegisterCompanyDTO body, CancellationToken cancellationToken = default)
    {
        var securityInfo = GetSecurityInfo(Request);

        await new CompanyRegistrationValidator().ValidateAndThrowAsync(body);

        var company = await _companyService.RegisterAsync(
            body.ToOwnerModel(),
            body.ToCompanyModel(),
            securityInfo,
            body.ToDocumentsMap(),
            cancellationToken);

        return Ok(PublicCompanyDTO.ModelToDTO(company));
    }

    [AuthAttribute]
    [Filters.Authorize(ProfileType.CLIENT, ProfileType.ADMIN, ProfileType.PLATFORM_DEVELOPER)]
    [HttpGet("me")]
    public async Task<IActionResult> MyCompanies(CancellationToken cancellationToken = default)
    {
        var userId = Authenticated!.User.Id;
        var companies = await _companyService.GetForUserAsync(userId, cancellationToken);
        return Ok(PublicCompanyDTO.ModelToDTO(companies));
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.CompanyRead)]
    [HttpGet("{companyId}")]
    public async Task<IActionResult> GetById(string companyId, CancellationToken cancellationToken = default)
    {
        var company = await _companyService.GetByIdAsync(companyId, cancellationToken);
        return Ok(PublicCompanyDTO.ModelToDTO(company));
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.CompanyUpdate)]
    [HttpPut("{companyId}")]
    public async Task<IActionResult> Update(string companyId, [FromBody] UpdateCompanyDTO body, CancellationToken cancellationToken = default)
    {
        var actorId = Authenticated!.User.Id;
        var company = await _companyService.UpdateAsync(companyId, actorId, body.ToModel(), cancellationToken);
        return Ok(PublicCompanyDTO.ModelToDTO(company));
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.CompanyUpdate)]
    [HttpPatch("{companyId}/configuration")]
    public async Task<IActionResult> UpdateConfiguration(
        string companyId,
        [FromBody] UpdateCompanyConfigurationDTO body,
        CancellationToken cancellationToken = default)
    {
        var actorId = Authenticated!.User.Id;
        var company = await _companyService.UpdateConfigurationAsync(companyId, actorId, body.ToModel(), cancellationToken);
        return Ok(PublicCompanyDTO.ModelToDTO(company));
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.CompanyRead)]
    [HttpGet("{companyId}/documents")]
    public async Task<IActionResult> GetDocuments(string companyId, CancellationToken cancellationToken = default)
    {
        var docs = await _companyService.GetDocumentsAsync(companyId, cancellationToken);
        return Ok(docs.Select(d => new
        {
            d.Id,
            Type = d.Type.ToString(),
            d.FileName,
            d.FileUrl,
            d.CreatedAt,
        }));
    }

    // Platform admin (PLATFORM_DEVELOPER): approval workflow
    [AuthAttribute]
    [Filters.Authorize(ProfileType.PLATFORM_DEVELOPER)]
    [HttpGet("admin/pending")]
    public async Task<IActionResult> GetPending(CancellationToken cancellationToken = default)
    {
        var pending = await _companyService.GetByApprovalStatusAsync(CompanyApprovalStatus.PENDING, cancellationToken);
        return Ok(PublicCompanyDTO.ModelToDTO(pending));
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.CompanyApprove)]
    [HttpPost("admin/{companyId}/approve")]
    public async Task<IActionResult> Approve(string companyId, CancellationToken cancellationToken = default)
    {
        var company = await _companyService.ApproveAsync(companyId, Authenticated!.User.Id, cancellationToken);
        return Ok(PublicCompanyDTO.ModelToDTO(company));
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.CompanyReject)]
    [HttpPost("admin/{companyId}/reject")]
    public async Task<IActionResult> Reject(string companyId, [FromBody] RejectCompanyDTO body, CancellationToken cancellationToken = default)
    {
        var company = await _companyService.RejectAsync(companyId, Authenticated!.User.Id, body.Reason, cancellationToken);
        return Ok(PublicCompanyDTO.ModelToDTO(company));
    }

    // Members
    [AuthAttribute]
    [HasPermission(PermissionKey.MemberList)]
    [HttpGet("{companyId}/members")]
    public async Task<IActionResult> GetMembers(string companyId, CancellationToken cancellationToken = default)
    {
        var rows = await _companyService.GetMembersDetailedAsync(companyId, cancellationToken);
        return Ok(rows.Select(t => CompanyMemberDetailDTO.ModelToDTO(t.member, t.user, t.role)).ToList());
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.MemberList)]
    [HttpGet("{companyId}/members/export")]
    public async Task<IActionResult> ExportMembersCsv(string companyId, CancellationToken cancellationToken = default)
    {
        var rows = await _companyService.GetMembersDetailedAsync(companyId, cancellationToken);

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Nome,Email,Cargo,Proprietário,Última atividade,Membro desde");

        static string Esc(string? s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            var needsQuote = s.Contains(',') || s.Contains('"') || s.Contains('\n');
            var escaped = s.Replace("\"", "\"\"");
            return needsQuote ? $"\"{escaped}\"" : escaped;
        }

        foreach (var (member, user, role) in rows)
        {
            sb.Append(Esc(user.Name)).Append(',');
            sb.Append(Esc(user.Email)).Append(',');
            sb.Append(Esc(role?.Name ?? string.Empty)).Append(',');
            sb.Append(member.IsOwner ? "Sim" : "Não").Append(',');
            sb.Append(user.LastAccessAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty).Append(',');
            sb.AppendLine(member.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        var bom = new byte[] { 0xEF, 0xBB, 0xBF };
        var bytes = bom.Concat(System.Text.Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        var fileName = $"equipe-{companyId}-{DateTime.UtcNow:yyyyMMddHHmmss}.csv";

        return File(bytes, "text/csv; charset=utf-8", fileName);
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.MemberList)]
    [HttpGet("{companyId}/invitations")]
    public async Task<IActionResult> GetInvitations(string companyId, CancellationToken cancellationToken = default)
    {
        var invites = await _companyService.GetInvitationsAsync(companyId, cancellationToken);
        return Ok(invites.Select(CompanyInvitationDTO.ModelToDTO).ToList());
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.MemberInvite)]
    [HttpPost("{companyId}/invitations/{invitationId}/resend")]
    public async Task<IActionResult> ResendInvitation(string companyId, string invitationId, CancellationToken cancellationToken = default)
    {
        var invite = await _companyService.ResendInvitationAsync(companyId, invitationId, Authenticated!.User.Id, cancellationToken);
        return Ok(CompanyInvitationDTO.ModelToDTO(invite));
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.MemberDelete)]
    [HttpDelete("{companyId}/invitations/{invitationId}")]
    public async Task<IActionResult> RevokeInvitation(string companyId, string invitationId, CancellationToken cancellationToken = default)
    {
        await _companyService.RevokeInvitationAsync(companyId, invitationId, Authenticated!.User.Id, cancellationToken);
        return NoContent();
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.MemberInvite)]
    [HttpPost("{companyId}/members")]
    public async Task<IActionResult> InviteMember(string companyId, [FromBody] InviteMemberDTO body, CancellationToken cancellationToken = default)
    {
        var member = await _companyService.InviteMemberAsync(companyId, body.UserEmail, body.RoleId, Authenticated!.User.Id, cancellationToken);
        return Ok(CompanyMemberDTO.ModelToDTO(member));
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.MemberDelete)]
    [HttpDelete("{companyId}/members/{userId}")]
    public async Task<IActionResult> RemoveMember(string companyId, string userId, CancellationToken cancellationToken = default)
    {
        await _companyService.RemoveMemberAsync(companyId, userId, Authenticated!.User.Id, cancellationToken);
        return NoContent();
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.MemberUpdateRole)]
    [HttpPut("{companyId}/members/{userId}/role")]
    public async Task<IActionResult> UpdateMemberRole(string companyId, string userId, [FromBody] AssignRoleDTO body, CancellationToken cancellationToken = default)
    {
        var member = await _companyService.AssignRoleAsync(companyId, userId, body.RoleId, Authenticated!.User.Id, cancellationToken);
        return Ok(CompanyMemberDTO.ModelToDTO(member));
    }

    // ─── Worker invitations ───────────────────────────────────────────────

    // The company owner/admin creates an invitation; the returned token is what
    // populates the future `/workers/accept?token=...` registration link.
    [AuthAttribute]
    [HasPermission(PermissionKey.MemberInvite)]
    [HttpPost("{companyId}/workers/invitations")]
    public async Task<IActionResult> CreateWorkerInvitation(
        string companyId,
        [FromBody] CreateWorkerInvitationDTO body,
        CancellationToken cancellationToken = default)
    {
        var invite = await _companyService.CreateWorkerInvitationAsync(
            companyId, body.Email, body.RoleKey, Authenticated!.User.Id, cancellationToken);
        return Ok(CompanyInvitationDTO.ModelToDTO(invite));
    }

    // Public lookup so the registration page can display the company / role
    // before the user fills the form.
    [HttpGet("workers/invitations/{token}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetWorkerInvitation(string token, CancellationToken cancellationToken = default)
    {
        var invite = await _companyService.GetInvitationByTokenAsync(token, cancellationToken);
        var company = await _companyService.GetByIdAsync(invite.CompanyId, cancellationToken);
        return Ok(new
        {
            invite.Email,
            invite.RoleKey,
            invite.ExpiresAt,
            Company = new
            {
                company.Id,
                company.FantasyName,
                company.SocialReason,
            },
        });
    }

    // Real onboarding flow: invited user creates their account against a token.
    [HttpPost("workers/register")]
    [AllowAnonymous]
    public async Task<IActionResult> AcceptWorkerInvitation(
        [FromBody] AcceptWorkerInvitationDTO body,
        CancellationToken cancellationToken = default)
    {
        var securityInfo = GetSecurityInfo(Request);

        var (user, _) = await _companyService.AcceptWorkerInvitationAsync(
            body.Token,
            new Domain.Data.Entities.User
            {
                Name = body.Name,
                Email = body.Email,
                Cellphone = body.Cellphone,
                Document = body.Document,
                Password = body.Password,
            },
            securityInfo,
            cancellationToken);

        return Ok(PublicUserDTO.ModelToDTO(user));
    }

    // Dev/test-only: skip the invite flow and create a worker directly so the
    // owner can seed worker accounts before the invite UX exists.
    [HttpPost("workers/register-direct")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterWorkerDirect(
        [FromBody] RegisterWorkerDirectDTO body,
        CancellationToken cancellationToken = default)
    {
        var securityInfo = GetSecurityInfo(Request);

        var (user, _) = await _companyService.RegisterWorkerDirectAsync(
            body.CompanyId,
            body.RoleKey,
            new Domain.Data.Entities.User
            {
                Name = body.Name,
                Email = body.Email,
                Cellphone = body.Cellphone,
                Document = body.Document,
                Password = body.Password,
            },
            securityInfo,
            cancellationToken);

        return Ok(PublicUserDTO.ModelToDTO(user));
    }
}
