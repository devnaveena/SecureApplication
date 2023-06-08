using Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public string? Role { get; set; }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Identity!.IsAuthenticated)
        {
            throw new UnauthorizedException("Access denied");
        }

        if (!string.IsNullOrEmpty(Role))
        {
            var roles = Role.Split(',');
            foreach (var role in roles)
            {
                if (context.HttpContext.User.IsInRole(role.Trim()))
                    return;
            }
            throw new ForbiddenException("Access denied");
        }
    }
}
