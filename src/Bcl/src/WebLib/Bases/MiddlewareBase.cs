﻿using System.Diagnostics;
using System.Security.Claims;

using Library.EventsArgs;

namespace Library.Web.Bases;

public abstract class MiddlewareBase(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    protected ClaimsPrincipal? User { get; private set; }

    [DebuggerStepThrough]
    public async Task Invoke(HttpContext httpContext)
    {
        var onExecutingArgs = new ItemActingEventArgs<HttpContext>(httpContext);
        await this.OnExecutingAsync(onExecutingArgs);
        if (!onExecutingArgs.Handled)
        {
            try
            {
                await this._next(httpContext);
            }
            catch (Exception ex)
            {
                var onExceptionOccurredArgs = new ItemActingEventArgs<Exception>(ex);
                await this.ExceptionOccurred(onExceptionOccurredArgs);
                if (!onExceptionOccurredArgs.Handled)
                {
                    throw;
                }
            }
            finally
            {
                var onExecutedArgs = new ItemActedEventArgs<HttpContext>(httpContext);
                await this.OnExecutedAsync(onExecutedArgs);
            }
        }
    }

    protected virtual Task ExceptionOccurred(ItemActingEventArgs<Exception> e)
        => Task.CompletedTask;

    protected virtual Task OnExecutedAsync(ItemActedEventArgs<HttpContext> e)
        => Task.CompletedTask;

    protected abstract Task OnExecutingAsync(ItemActingEventArgs<HttpContext> e);
}