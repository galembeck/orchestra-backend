using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.Filters;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Public.Controllers;

[ApiController]
[Route("service-categories")]
public class ServiceCategoryController(
    IServiceCategoryService serviceCategoryService,
    IHttpContextAccessor httpContextAccessor) : _BaseController(httpContextAccessor)
{
    [AuthAttribute]
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken = default)
    {
        var categories = await serviceCategoryService.ListActiveAsync(cancellationToken);
        return Ok(ServiceCategoryDTO.ModelToDTO(categories));
    }
}
