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
        var members = await _companyService.GetMembersAsync(companyId, cancellationToken);
        return Ok(CompanyMemberDTO.ModelToDTO(members));
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
}
