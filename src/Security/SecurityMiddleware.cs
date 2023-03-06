using Library.EventsArgs;
using Library.Web.Bases;

using Microsoft.AspNetCore.Http;

namespace HanyCo.Infra.Security;

internal class SecurityMiddleware : MiddlewareBase
{
    public SecurityMiddleware(RequestDelegate next) : base(next)
    {
    }

    protected override Task OnExecutingAsync(ItemActingEventArgs<HttpContext?> e)
        => Task.CompletedTask;
}