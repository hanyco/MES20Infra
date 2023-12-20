using System.Collections.Immutable;
using System.Security.Claims;

using HanyCo.Infra.Security.Identity.AuthorizationStrategies;

using Library.EventsArgs;
using Library.Web.Bases;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace HanyCo.Infra.Security;

internal class SecurityMiddleware(RequestDelegate next, IAuthorizationService authorization) : MiddlewareBase(next)
{
    protected override Task OnExecutingAsync(ItemActingEventArgs<HttpContext> e)
    {
        if (e.Item == default)
        {
            return Task.CompletedTask;
        }
        if (this.User != default)
        {
            if (this.IsAdminUser(this.User))
            {
                return Task.CompletedTask;
            }
        }

        var route = GetRoute(e.Item);

        //if (localPath.IsNullOrEmpty() || localPath == "\\" || localPath == "/" || localPath.StartsWith("/_blazor"))
        //{
        //    return Task.CompletedTask;
        //}

        var claims = PolicyManager.GetClaimsByRoute(route.LocalPath.ToString()).ToImmutableArray();
        if (!claims.Any())
        {
            return Task.CompletedTask;
        }
        if (this.User == default)
        {
            e.Item.Abort();
            e.Handled = true;
            return Task.CompletedTask;
        }
        var violations = claims.Where(x => !this.User.HasClaim(x.ClaimType, x.ClaimValue));
        //var authorizationResult = await authorization.AuthorizeAsync(this.User, claims);
        //if (!authorizationResult.Succeeded)
        if (violations.Any())
        {
            e.Item.Abort();
            e.Handled = true;
            return Task.CompletedTask;
        }
        return Task.CompletedTask;
    }

    private static Uri GetRoute(HttpContext context) =>
        new(context.Request.GetEncodedUrl());

    private bool IsAdminUser(ClaimsPrincipal user)
    {
        IEnumerable<string> adminRoles = [
                "Admin",
                "Admins",
                "Administrator",
                "Administrators"
            ];
        return adminRoles.Any(user.IsInRole);
    }
}