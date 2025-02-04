﻿using System.Diagnostics;

using Library.DesignPatterns.Markers;
using Library.Logging;
using Library.Results;
using Library.Validations;

namespace Library.Coding;

[Immutable]
[DebuggerStepThrough, StackTraceHidden]
public sealed class ActionScope : IDisposable
{
    public const string BEGININGMESSAGE = null;
    public const string ENDINGMESSAGE = "Ready";
    private readonly Action<string> _prompt;
    private bool _isEnded;

    private ActionScope(Action<string> prompt) =>
        this._prompt = prompt.ArgumentNotNull();

    public static ActionScope Begin(Action<string> prompt, string message = BEGININGMESSAGE) =>
        new ActionScope(prompt).Begin(message);

    public static ActionScope Begin(ILogger logger, string message = BEGININGMESSAGE) =>
        new ActionScope(ActionScopeExtensions.ToAction(logger)).Begin(message);

    public static ActionScope Begin(ILoggerContainer logger, string message = BEGININGMESSAGE) =>
        new ActionScope(ActionScopeExtensions.ToAction(logger)).Begin(message);

    public static ActionScope New(Action<string> prompt) =>
        new(prompt);

    public static ActionScope New(ILogger logger) =>
        new(ActionScopeExtensions.ToAction(logger));

    public static ActionScope New(ILoggerContainer logger) =>
        new(ActionScopeExtensions.ToAction(logger));

    public static TResult Run<TResult>(ILogger logger, Func<TResult> action, string beginMessage = BEGININGMESSAGE, string endMessage = ENDINGMESSAGE)
        where TResult : ResultBase
    {
        TResult result;
        using (var actionScope = Begin(ActionScopeExtensions.ToAction(logger), beginMessage))
        {
            result = actionScope.Do(action).With(x => x.End(x.Result.Message ?? endMessage)).Result;
        }
        return result;
    }

    public ActionScope Begin(string message = BEGININGMESSAGE) =>
        this.Prompt(message);

    public void Dispose() =>
        this.End();

    public void End(string message = ENDINGMESSAGE)
    {
        Check.ThrowIfDisposed(this, this._isEnded);

        _ = this.Prompt(message);
        this._isEnded = true;
    }

    public ActionScope Prompt(string message)
    {
        this._prompt(message);
        return this;
    }

    public object Run(Action simpleMethod) => throw new NotImplementedException();
}

public static class ActionScopeExtensions
{
    public static ActionScope ActionScopeBegin(this ILoggerContainer loggerContainer, string beginMessage = ActionScope.BEGININGMESSAGE)
        => ActionScope.Begin(x => loggerContainer.Debug(x), beginMessage);

    public static void ActionScopeRun(this ILogger logger, Action action, string beginMessage = ActionScope.BEGININGMESSAGE, string endMessage = ActionScope.ENDINGMESSAGE)
    {
        var scope = ActionScope.Begin(ToAction(logger), beginMessage);
        action();
        scope.End(endMessage);
    }

    public static TResult ActionScopeRun<TResult>(this ILogger logger, Func<TResult> action, string beginMessage = ActionScope.BEGININGMESSAGE, string endMessage = ActionScope.ENDINGMESSAGE)
    {
        var scope = ActionScope.Begin(ToAction(logger), beginMessage);
        var result = action();
        scope.End(endMessage);
        return result;
    }

    public static TResult ActionScopeRun<TResult>(this ILoggerContainer loggerContainer, Func<TResult> action, string beginMessage = ActionScope.BEGININGMESSAGE, string endMessage = ActionScope.ENDINGMESSAGE)
    {
        var scope = ActionScope.Begin(ToAction(loggerContainer), beginMessage);
        var result = action();
        scope.End(endMessage);
        return result;
    }

    public static Task<TResult> ActionScopeRunAsync<TResult>(this ILoggerContainer logger, Func<Task<TResult>> action, string beginMessage = ActionScope.BEGININGMESSAGE, string endMessage = ActionScope.ENDINGMESSAGE)
                        where TResult : ResultBase
    {
        var scope = ActionScope.Begin(x => logger.Debug(x), beginMessage);
        return action().WithAsync(x => scope.End(x));
    }

    public static async Task ActionScopeRunAsync(this ILogger logger, Func<Task> action, string beginMessage = ActionScope.BEGININGMESSAGE, string endMessage = ActionScope.ENDINGMESSAGE)
    {
        var scope = ActionScope.Begin(ToAction(logger), beginMessage);
        await action().ConfigureAwait(false);
        scope.End(endMessage);
    }

    public static async Task ActionScopeRunAsync(this ILoggerContainer loggerContainer, Func<Task> action, string beginMessage = ActionScope.BEGININGMESSAGE, string endMessage = ActionScope.ENDINGMESSAGE)
    {
        var scope = ActionScope.Begin(ToAction(loggerContainer), beginMessage);
        await action().ConfigureAwait(false);
        scope.End(endMessage);
    }

    public static (TResult Result, ActionScope Scope) Do<TResult>(this ActionScope scope, Func<TResult> action, string beginMessage = ActionScope.BEGININGMESSAGE, string endMessage = ActionScope.ENDINGMESSAGE)
    {
        _ = scope.Begin(beginMessage);
        var result = action();
        return (result, scope);
    }

    public static void End<TResult>(this (TResult Result, ActionScope Scope) result, string endMessage = ActionScope.ENDINGMESSAGE)
        where TResult : ResultBase =>
        result.Scope.End(result.Result, endMessage);

    public static void End(this ActionScope scope, ResultBase result, string endMessage = ActionScope.ENDINGMESSAGE) =>
        scope.End(result.Message ?? endMessage);

    public static void EndActionScope(this ILogger logger, string endMessage = ActionScope.ENDINGMESSAGE) =>
        ToAction(logger)(ActionScope.ENDINGMESSAGE);

    public static void EndActionScope(this ILoggerContainer logger, string endMessage = ActionScope.ENDINGMESSAGE) =>
        ToAction(logger)(ActionScope.ENDINGMESSAGE);

    public static Result? EndActionScope(this ILoggerContainer logger, Result? result, string endMessage = ActionScope.ENDINGMESSAGE)
    {
        ToAction(logger)(result?.Message ?? endMessage);
        return result;
    }

    public static Result<TResult>? EndActionScope<TResult>(this ILoggerContainer logger, Result<TResult>? result, string endMessage = ActionScope.ENDINGMESSAGE)
    {
        ToAction(logger)(result?.Message ?? endMessage);
        return result;
    }

    public static ActionScope GetActionScope(this ILoggerContainer loggerContainer) =>
        loggerContainer.props().ActionScope.Cast().As<ActionScope>() ?? (loggerContainer.props().ActionScope = ActionScope.New(loggerContainer));

    public static TResult RunActionScope<TResult>(this ILogger logger, Func<TResult> action, string beginMessage = ActionScope.BEGININGMESSAGE, string endMessage = ActionScope.ENDINGMESSAGE)
        where TResult : ResultBase =>
        ActionScope
                .Begin(ToAction(logger), beginMessage)
                .Do(action)
                .With(x => x.End(x.Result.Message ?? endMessage)).Result;

    internal static Action<string> ToAction(ILogger logger) =>
        x => logger.Debug(x);

    internal static Action<string> ToAction(ILoggerContainer logger) =>
        x => logger.Debug(x);
}