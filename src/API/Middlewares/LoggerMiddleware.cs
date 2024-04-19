using System.Diagnostics;

using HanyCo.Infra.Web.Middlewares;

namespace API.Middlewares;

public class LoggerMiddleware(RequestDelegate next, ILogger<LoggerMiddleware> logger):MesMiddlewareBase
{
    private readonly ILogger<LoggerMiddleware> _logger = logger;
    private readonly RequestDelegate _next = next;

    public Task Invoke(HttpContext httpContext)
    {
        var timer = Stopwatch.StartNew();
        try
        {
            this._logger.LogDebug("Executing {api}", httpContext.Request.Path);
            return this._next(httpContext);
        }
        finally
        {
            timer.Stop();
            this._logger.LogTrace("Executed {api} in {elapsed}", httpContext.Request.Path, timer.Elapsed);
        }
    }
}

public static class LoggerMiddlewareExtensions
{
    public static IApplicationBuilder UseLoggerMiddleware(this IApplicationBuilder builder)
        => builder.UseMiddleware<LoggerMiddleware>();
}