using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.Filters;
using Domain.Constants;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Public.Controllers;

[ApiController]
[Route("[controller]")]
public class ServiceController(
    IServiceManagementService serviceManagementService,
    IHttpContextAccessor httpContextAccessor) : _BaseController(httpContextAccessor)
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ListAll(CancellationToken cancellationToken = default)
    {
        var rows = await serviceManagementService.ListAllAsync(cancellationToken);
        return Ok(PublicServiceDTO.ModelToDTO(rows));
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken = default)
    {
        var (s, c, co) = await serviceManagementService.GetByIdAsync(id, cancellationToken);
        return Ok(PublicServiceDTO.ModelToDTO(s, c, co));
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.ServiceList)]
    [HttpGet("company/{companyId}")]
    public async Task<IActionResult> ListByCompany(string companyId, CancellationToken cancellationToken = default)
    {
        var rows = await serviceManagementService.ListByCompanyAsync(companyId, cancellationToken);
        return Ok(PublicServiceDTO.ModelToDTO(rows));
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.ServiceCreate)]
    [HttpPost("company/{companyId}")]
    public async Task<IActionResult> Create(
        string companyId,
        [FromBody] CreateServiceDTO body,
        CancellationToken cancellationToken = default)
    {
        var actorId = Authenticated!.User.Id;
        var service = await serviceManagementService.CreateAsync(companyId, actorId, body.ToModel(), cancellationToken);
        var (s, cat, co) = await serviceManagementService.GetByIdAsync(service.Id, cancellationToken);
        return Ok(PublicServiceDTO.ModelToDTO(s, cat, co));
    }
}
