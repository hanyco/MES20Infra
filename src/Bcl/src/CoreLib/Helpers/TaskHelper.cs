using Library.Exceptions;
using Library.Results;
using Library.Validations;

using System.Diagnostics;

namespace Library.Helpers;

[DebuggerStepThrough, StackTraceHidden]
public static class TaskHelper
{
    /// <summary>
    /// Invokes an action asynchronously.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    /// <param name="scheduler">The scheduler to use for the task.</param>
    /// <param name="token">The cancellation token to use for the task.</param>
    public static async void InvokeAsync(this Action action, TaskScheduler? scheduler = null, CancellationToken token = default)
    {
        Check.MustBeArgumentNotNull(action);

        if (scheduler != null)
        {
            await Task.Factory.StartNew(action, token, TaskCreationOptions.None, scheduler);
        }
        else
        {
            await Task.Factory.StartNew(action, token, TaskCreationOptions.None, TaskScheduler.Default);
        }
    }

    public static async Task<Result> RunAllAsync(this IEnumerable<Func<Task<Result>>> funcs, CancellationToken token)
    {
        Check.MustBeArgumentNotNull(funcs);

        foreach (var func in funcs.WithCancellation(token))
        {
            var result = await func();
            if (result.IsFailure)
            {
                return result;
            }
        }
        return token.IsCancellationRequested
            ? Result.Fail<OperationCancelledException>()
            : Result.Succeed;
    }

    public static async Task<Result> RunAllAsync<TState>(this IEnumerable<Func<TState, Task<Result>>> funcs, TState initialState, CancellationToken token = default)
    {
        Check.MustBeArgumentNotNull(funcs);

        foreach (var func in funcs.WithCancellation(token))
        {
            var result = await func(initialState);
            if (result.IsFailure)
            {
                return result;
            }
        }
        return token.IsCancellationRequested
            ? Result.Fail<OperationCancelledException>()
            : Result.Succeed;
    }

    public static async Task<Result> RunAllAsync<TState>(this IEnumerable<Func<TState, CancellationToken, Task<Result>>> funcs, TState initialState, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(funcs);

        foreach (var func in funcs.WithCancellation(cancellationToken))
        {
            var result = await func(initialState, cancellationToken);
            if (result.IsFailure)
            {
                return result;
            }
        }
        return cancellationToken.IsCancellationRequested
            ? Result.Fail<OperationCancelledException>()
            : Result.Succeed;
    }

    public static Task ThrowIfCancellationRequested(this Task task, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return task;
    }

    public static Task<T> ThrowIfCancellationRequested<T>(this Task<T> task, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return task;
    }

    /// <summary>
    /// Creates a task that will complete when all of the <see cref="Task"/> objects in an array
    /// have completed.
    /// </summary>
    /// <remarks>Returns all the exceptions occurred, if any</remarks>
    /// <typeparam name="TResult">The type of the completed task.</typeparam>
    /// <param name="tasks">The tasks to wait on for completion.</param>
    /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
    public static async Task<IEnumerable<TResult>> WhenAll<TResult>(params IEnumerable<Task<TResult>> tasks)
    {
        var allTasks = Task.WhenAll(tasks);
        return await allTasks;
    }

    /// <summary>
    /// Creates a task that will complete when all of the <see cref="Task"/> objects in an array
    /// have completed.
    /// </summary>
    /// <remarks>Returns all the exceptions occurred, if any</remarks>
    /// <typeparam name="TResult">The type of the completed task.</typeparam>
    /// <param name="tasks">The tasks to wait on for completion.</param>
    /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
    public static Task WhenAll(params IEnumerable<Task> tasks) =>
        Task.WhenAll(tasks);

}