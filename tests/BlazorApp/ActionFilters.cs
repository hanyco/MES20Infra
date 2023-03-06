using Microsoft.AspNetCore.Mvc.Filters;

namespace BlazorApp;

public class LoggingResponseHeaderFilterService : IResultFilter
{
    private readonly ILogger _logger;

    public LoggingResponseHeaderFilterService(
            ILogger<LoggingResponseHeaderFilterService> logger) =>
        this._logger = logger;

    public void OnResultExecuted(ResultExecutedContext context) => this._logger.LogInformation(
            $"- {nameof(LoggingResponseHeaderFilterService)}.{nameof(OnResultExecuted)}");

    public void OnResultExecuting(ResultExecutingContext context)
    {
        this._logger.LogInformation(
            $"- {nameof(LoggingResponseHeaderFilterService)}.{nameof(OnResultExecuting)}");

        context.HttpContext.Response.Headers.Add(
            nameof(OnResultExecuting), nameof(LoggingResponseHeaderFilterService));
    }
}

public class PageFilter : IPageFilter
{
    public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
    }

    public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
    }

    public void OnPageHandlerSelected(PageHandlerSelectedContext context)
    {
    }
}

public class SampleActionFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Do something after the action executes.
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Do something before the action executes.
    }
}

public class SampleAsyncPageFilter : IAsyncPageFilter
{
    public SampleAsyncPageFilter()
    {

    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next) =>
        // Do post work.
        await next.Invoke();

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        return Task.CompletedTask;
    }
}