using Library.EventsArgs;
using Library.Web.Bases;
using Library.Web.Middlewares.Markers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace HanyCo.Infra.Security;

[MonitoringMiddleware]
internal class SecurityMiddleware(RequestDelegate next, IAuthorizationService authorization) : MiddlewareBase(next)
{
    protected override Task OnExecutingAsync(ItemActingEventArgs<HttpContext> e) =>
        Task.CompletedTask;

    private static Uri GetRoute(HttpContext context) =>
        new(context.Request.GetEncodedUrl());
}