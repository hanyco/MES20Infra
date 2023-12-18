using Library.EventsArgs;
using Library.Web.Middlewares.Markers;

using Microsoft.AspNetCore.Http;

namespace HanyCo.Infra.Web.Middlewares;

[ShortCircuitMiddleware]
public sealed class LicenseManagerMiddleware(RequestDelegate next) : InfraMiddlewareBase(next), IInfraMiddleware
{
    protected override async Task OnExecutingAsync(ItemActingEventArgs<HttpContext> e)
        => await Task.CompletedTask;
}