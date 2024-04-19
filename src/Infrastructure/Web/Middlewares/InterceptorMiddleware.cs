using Library.EventsArgs;
using Library.Web.Middlewares.Markers;

using Microsoft.AspNetCore.Http;

namespace HanyCo.Infra.Web.Middlewares;

[ContentGeneratorMiddleware]
public sealed class InterceptorMiddleware(RequestDelegate next) : MesMiddlewareBase(next)
{
    protected override async Task OnExecutingAsync(ItemActingEventArgs<HttpContext> e)
    {
        var httpContext = e.Item;
        var path = httpContext?.Request.Path.ToString().Trim().ToLower();
        if (path?.EndsWith("/infra") is true && httpContext is { } http)
        {
            await http.Response.WriteAsync("<h1><center>MES Infrastructure is up.</center></h1>", Encoding.UTF8);
        }
    }
}