using System.Security.Claims;

using Application.Common.Enums;
using Application.Features.Permissions.Services;

using HanyCo.Infra.Security;

using Microsoft.AspNetCore.Authorization;

namespace API.Middlewares;

[Obsolete("Extension methods class. Do NOT use, directly.", true)]
public static class AccessControlMiddlewareExtensions
{
    public static IApplicationBuilder UsePermissionMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<PermissionMiddleware>();
}

public class PermissionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            var attributes = endpoint.Metadata.GetOrderedMetadata<PermissionAttribute>();
            if (attributes.Any())
            {
                // Retrieve user permissions
                var userPermissions = context.User.Claims
                    .FirstOrDefault(c => c.Type == "Permissions")?.Value.Split(',');

                // Check if user has any of the required permissions
                foreach (var attribute in attributes)
                {
                    if (userPermissions == null || !userPermissions.Contains(attribute.Permission))
                    {
                        // User does not have the required permission
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Unauthorized access.");
                        return;
                    }
                }
            }
        }

        await _next(context);
    }
}