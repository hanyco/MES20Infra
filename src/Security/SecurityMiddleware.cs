using HanyCo.Infra.Security.Identity.AuthorizationStrategies;

using Library.EventsArgs;
using Library.Web.Bases;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace HanyCo.Infra.Security;

internal class SecurityMiddleware(RequestDelegate next, IAuthorizationService authorization) : MiddlewareBase(next)
{
    protected override async Task OnExecutingAsync(ItemActingEventArgs<HttpContext?> e)
    {
        if (e.Item == default)
        {
            return;
        }
        var localPath = new Uri(e.Item.Request.GetEncodedUrl()).LocalPath;
        if (localPath.IsNullOrEmpty() || localPath == "\\" || localPath == "/")
        {
            return;
        }

        var policy = PolicyManager.GetPolicyNameByUrl(localPath);
        if (policy.IsNullOrEmpty())
        {
            return;
        }
        if (this.User == default)
        {
            e.Item.Abort();
            e.Handled = true;
            return;
        }

        var authorizationResult = await authorization.AuthorizeAsync(this.User, policy);
        if (!authorizationResult.Succeeded)
        {
            e.Item.Abort();
            e.Handled = true;
        }
    }
}