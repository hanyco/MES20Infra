using Library.EventsArgs;
using Library.Web.Middlewares.Markers;

using Microsoft.AspNetCore.Http;

namespace HanyCo.Infra.Web.Middlewares;

[ShortCircuitMiddleware]
public sealed class LicenseManagerMiddleware(RequestDelegate next) : MesMiddlewareBase(next), Library.Web.Middlewares.Markers.IMiddleware
{
    protected override async Task OnExecutingAsync(ItemActingEventArgs<HttpContext> e)
        => await Task.CompletedTask;
}