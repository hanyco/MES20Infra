using Application.Common.Enums;
using Application.Features.Permissions.Services;

using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;

namespace API.Middlewares;

public class AccessControlMiddleware(RequestDelegate next, ILogger<AccessControlMiddleware> logger, IAccessControlService accessControlService)
{
    private readonly IAccessControlService _accessControlService = accessControlService;
    private readonly ILogger<AccessControlMiddleware> _logger = logger;
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        // Allow requests with [AllowAnonymous] attribute to pass without authorization check
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            await _next(context);
            return;
        }

        // Step 1: Check authentication
        if (!context.User.Identity.IsAuthenticated)
        {
            _logger.LogWarning("Unauthenticated request.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        // Step 2: Retrieve necessary information from the request
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var path = context.Request.Path.ToString().ToLower();

        // Step 3: Check access permissions using the Access Control Service
        var accessLevel = await _accessControlService.GetAccessLevel(userId, path);
        if (accessLevel == AccessLevel.NoAccess)
        {
            _logger.LogWarning($"User {userId} is not authorized to access {path}");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden");
            return;
        }

        // Step 4: Allow read-only access if the user has read-only permissions
        if (context.Request.Method != HttpMethods.Get && accessLevel == AccessLevel.ReadOnly)
        {
            _logger.LogWarning($"User {userId} has read-only access to {path}, but attempted {context.Request.Method}");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden - Read-only access");
            return;
        }

        // Proceed if the user has sufficient access
        await _next(context);
    }
}

public static class AccessControlMiddlewareExtensions
{
    public static IApplicationBuilder UseAccessControlMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<AccessControlMiddleware>();
}
