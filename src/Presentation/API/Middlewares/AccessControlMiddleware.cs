using Application.Common.Enums;
using Application.Features.Permissions.Services;

using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;

namespace API.Middlewares;

public static class AccessControlMiddlewareExtensions
{
    public static IApplicationBuilder UseAccessControlMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<AccessControlMiddleware>();
}

public class AccessControlMiddleware(RequestDelegate next, ILogger<AccessControlMiddleware> logger, IAccessControlService accessControlService)
{
    private readonly IAccessControlService _accessControlService = accessControlService;
    private readonly ILogger<AccessControlMiddleware> _logger = logger;
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            _logger.LogInformation("AllowAnonymous request. Path: {Path}, Method: {Method}",
                context.Request.Path, context.Request.Method);
            await _next(context);
            return;
        }

        if (!context.User.Identity.IsAuthenticated)
        {
            _logger.LogWarning("Unauthenticated request. Path: {Path}, Method: {Method}, IP: {IPAddress}",
                context.Request.Path, context.Request.Method, context.Connection.RemoteIpAddress);
            await writeResponse(context, StatusCodes.Status401Unauthorized, "Unauthorized");
            return;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var path = context.Request.Path.ToString().ToLower();

        var accessLevel = await _accessControlService.GetAccessLevel(userId, path);
        if (accessLevel == AccessLevel.NoAccess)
        {
            _logger.LogWarning("Access denied for user {UserId}. Path: {Path}, Method: {Method}",
                userId, path, context.Request.Method);
            await writeResponse(context, StatusCodes.Status403Forbidden, "Forbidden");
            return;
        }

        if (context.Request.Method != HttpMethods.Get && accessLevel == AccessLevel.ReadOnly)
        {
            _logger.LogWarning("Read-only access violation. User: {UserId}, Path: {Path}, Method: {Method}",
                userId, path, context.Request.Method);
            await writeResponse(context, StatusCodes.Status403Forbidden, "Forbidden - Read-only access");
            return;
        }

        _logger.LogInformation("Request allowed. User: {UserId}, Path: {Path}, Method: {Method}",
            userId, path, context.Request.Method);

        await _next(context);

        static async Task writeResponse(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(message);
        }
    }
}