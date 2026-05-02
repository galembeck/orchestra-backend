using API.Public.Controllers._Base;
using Domain.Constants;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Public.Filters;

// Verifies the authenticated user has `permission` for the company referenced
// by a route parameter (default: `companyId`). Must run AFTER [AuthAttribute].
public class HasPermissionAttribute : ActionFilterAttribute
{
    private readonly string _permission;
    private readonly string _routeParam;

    public HasPermissionAttribute(string permission, string routeParam = "companyId")
    {
        _permission = permission;
        _routeParam = routeParam;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = _BaseController.Authenticated?.User
            ?? throw new ForbiddenException();

        // Platform-only permissions (company:approve / company:reject) skip the
        // company-scoped lookup — only PLATFORM_DEVELOPER profile may exercise.
        if (PermissionKey.PlatformOnly.Contains(_permission))
        {
            if (user.ProfileType != ProfileType.PLATFORM_DEVELOPER)
                throw new ForbiddenException();
            await next();
            return;
        }

        if (!context.RouteData.Values.TryGetValue(_routeParam, out var raw) || raw is null)
            throw new ForbiddenException();

        var companyId = raw.ToString()!;

        var rbac = context.HttpContext.RequestServices.GetRequiredService<IRbacService>();
        var allowed = await rbac.HasPermissionAsync(user, companyId, _permission);

        if (!allowed)
            throw new ForbiddenException();

        await next();
    }
}
