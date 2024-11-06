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

        // Step 1: Allow requests with [AllowAnonymous]
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            await _next(context);
            return;
        }

        // Step 2: Check authentication
        if (!context.User.Identity.IsAuthenticated)
        {
            _logger.LogWarning("Unauthenticated request.");
            await WriteResponse(context, StatusCodes.Status401Unauthorized, "Unauthorized");
            return;
        }

        // Step 3: Retrieve information from the request
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var path = context.Request.Path.ToString().ToLower();

        // Step 4: Check access permissions
        var accessLevel = await _accessControlService.GetAccessLevel(userId, path);
        if (accessLevel == AccessLevel.NoAccess)
        {
            _logger.LogWarning($"User {userId} is not authorized to access {path}");
            await WriteResponse(context, StatusCodes.Status403Forbidden, "Forbidden");
            return;
        }

        // Step 5: Handle read-only access
        if (context.Request.Method != HttpMethods.Get && accessLevel == AccessLevel.ReadOnly)
        {
            _logger.LogWarning($"User {userId} attempted {context.Request.Method} on {path} with read-only access");
            await WriteResponse(context, StatusCodes.Status403Forbidden, "Forbidden - Read-only access");
            return;
        }

        // Step 6: Allow access
        await _next(context);
    }

    private static async Task WriteResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(message);
    }
}


public static class AccessControlMiddlewareExtensions
{
    public static IApplicationBuilder UseAccessControlMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<AccessControlMiddleware>();
}
