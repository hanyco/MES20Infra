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
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            await this._next(context);
            return;
        }

        // گام اول: بررسی احراز هویت
        if (!context.User.Identity.IsAuthenticated)
        {
            this._logger.LogWarning("Unauthenticated request.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        // گام دوم: استخراج اطلاعات مورد نیاز از درخواست
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var path = context.Request.Path.ToString().ToLower();

        // گام سوم: بررسی دسترسی با استفاده از سرویس کنترل دسترسی
        var isAuthorized = await this._accessControlService.HasAccess(userId, path);
        if (!isAuthorized)
        {
            this._logger.LogWarning($"User {userId} is not authorized to access {path}");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden");
            return;
        }

        // اگر دسترسی مجاز بود، ادامه بدهید
        await this._next(context);
    }
}

public static class AccessControlMiddlewareExtensions
{
    public static IApplicationBuilder UseAccessControlMiddleware(this IApplicationBuilder app) => app.UseMiddleware<AccessControlMiddleware>();
}