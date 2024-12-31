using System.Security.Claims;

using Application.Common.Enums;
using Application.Features.Permissions.Services;

using Microsoft.AspNetCore.Authorization;

namespace API.Middlewares;

[Obsolete("Extension methods class. Do NOT use, directly.", true)]
public static class AccessControlMiddlewareExtensions
{
    public static IApplicationBuilder UseAccessControlMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<AccessControlMiddleware>();
}

internal class AccessControlMiddleware
{
    private readonly IAccessControlService _accessControlService;
    private readonly ILogger<AccessControlMiddleware> _logger;
    private readonly RequestDelegate _next;

    public AccessControlMiddleware(RequestDelegate next, ILogger<AccessControlMiddleware> logger, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var accessControlService = scope.ServiceProvider.GetRequiredService<IAccessControlService>();
        this._accessControlService = accessControlService;
        this._logger = logger;
        this._next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await this._next(context);
            return;
        }

        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            this._logger.LogInformation("AllowAnonymous request. Path: {Path}, Method: {Method}",
                context.Request.Path, context.Request.Method);
            await this._next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated != true)
        {
            this._logger.LogWarning("Unauthenticated request. Path: {Path}, Method: {Method}, IP: {IPAddress}",
                context.Request.Path, context.Request.Method, context.Connection.RemoteIpAddress);
            await writeResponse(context, StatusCodes.Status401Unauthorized, "Unauthorized");
            return;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var path = context.Request.Path.ToString().ToLower();

        var accessLevel = await this._accessControlService.GetAccessLevel(userId, path);
        if (accessLevel == AccessLevel.NoAccess)
        {
            this._logger.LogWarning("Access denied for user {UserId}. Path: {Path}, Method: {Method}",
                userId, path, context.Request.Method);
            await writeResponse(context, StatusCodes.Status403Forbidden, "Forbidden");
            return;
        }

        if (context.Request.Method != HttpMethods.Get && accessLevel == AccessLevel.ReadOnly)
        {
            this._logger.LogWarning("Read-only access violation. User: {UserId}, Path: {Path}, Method: {Method}",
                userId, path, context.Request.Method);
            await writeResponse(context, StatusCodes.Status403Forbidden, "Forbidden - Read-only access");
            return;
        }

        this._logger.LogInformation("Request allowed. User: {UserId}, Path: {Path}, Method: {Method}", userId, path, context.Request.Method);

        await this._next(context);

        static async Task writeResponse(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(message);
        }
    }
}