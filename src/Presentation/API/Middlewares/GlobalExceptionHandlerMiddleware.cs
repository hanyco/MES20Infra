using Newtonsoft.Json;

namespace API.Middlewares;

public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandlerMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
}

public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger;
    private readonly RequestDelegate _next = next;
    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await this._next(httpContext);
        }
        catch (Exception ex)
        {
            await this.HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        this._logger.LogError(exception, "Unhandled exception occurred. Path: {Path}, Method: {Method}, User: {UserId}, IP: {IPAddress}", context.Request.Path, context.Request.Method, context.User.Identity?.Name ?? "Anonymous", context.Connection.RemoteIpAddress);

        var response = new { message = exception.Message, stackTrace = exception.StackTrace };
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
}
