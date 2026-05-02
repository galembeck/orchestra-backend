using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.Filters;
using Domain.Constants;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Public.Controllers;

[ApiController]
[Route("company/{companyId}/role")]
public class RoleController(
    IRoleService roleService,
    IHttpContextAccessor httpContextAccessor) : _BaseController(httpContextAccessor)
{
    private readonly IRoleService _roleService = roleService;

    [AuthAttribute]
    [HasPermission(PermissionKey.RoleRead)]
    [HttpGet]
    public async Task<IActionResult> List(string companyId, CancellationToken cancellationToken = default)
    {
        var roles = await _roleService.GetByCompanyAsync(companyId, cancellationToken);
        return Ok(RoleDTO.ModelToDTO(roles));
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.RoleRead)]
    [HttpGet("{roleId}/permissions")]
    public async Task<IActionResult> GetPermissions(string companyId, string roleId, CancellationToken cancellationToken = default)
    {
        var keys = await _roleService.GetPermissionKeysAsync(roleId, cancellationToken);
        return Ok(keys);
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.RoleCreate)]
    [HttpPost]
    public async Task<IActionResult> Create(string companyId, [FromBody] CreateRoleDTO body, CancellationToken cancellationToken = default)
    {
        var role = await _roleService.CreateAsync(
            companyId, body.Name, body.Description, body.Permissions,
            Authenticated!.User.Id, cancellationToken);

        return Ok(RoleDTO.ModelToDTO(role));
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.RoleUpdate)]
    [HttpPut("{roleId}")]
    public async Task<IActionResult> Update(string companyId, string roleId, [FromBody] UpdateRoleDTO body, CancellationToken cancellationToken = default)
    {
        var role = await _roleService.UpdateAsync(roleId, body.Name, body.Description, Authenticated!.User.Id, cancellationToken);
        return Ok(RoleDTO.ModelToDTO(role));
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.RoleUpdate)]
    [HttpPut("{roleId}/permissions")]
    public async Task<IActionResult> SetPermissions(string companyId, string roleId, [FromBody] SetRolePermissionsDTO body, CancellationToken cancellationToken = default)
    {
        await _roleService.SetPermissionsAsync(roleId, body.Permissions, Authenticated!.User.Id, cancellationToken);
        return NoContent();
    }

    [AuthAttribute]
    [HasPermission(PermissionKey.RoleDelete)]
    [HttpDelete("{roleId}")]
    public async Task<IActionResult> Delete(string companyId, string roleId, CancellationToken cancellationToken = default)
    {
        await _roleService.DeleteAsync(roleId, Authenticated!.User.Id, cancellationToken);
        return NoContent();
    }
}
