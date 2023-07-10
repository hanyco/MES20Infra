using Library.DesignPatterns.Markers;
using Library.Exceptions;
using Library.Results;
using Library.Validations;

namespace Library.Threading.MultistepProgress;

[Fluent]
public sealed class TaskRunner<TArg>
{
    private readonly List<Func<TArg, CancellationToken, Task<TArg>>> _funcList = new();
    private readonly Func<CancellationToken, Task<TArg>> _start;
    private bool _isRunning;
    private Action<Result<TArg?>>? _onEnded;

    private TaskRunner([DisallowNull] Func<CancellationToken, Task<TArg>> start, IEnumerable<Func<TArg, CancellationToken, Task<TArg>>>? funcs = null)
    {
        Check.IfArgumentNotNull(start);
        this._start = start;
        if (funcs?.Any() ?? false)
        {
            this._funcList.AddRange(funcs);
        }
    }

    public static Task<Result<TArg?>> RunAllAsync([DisallowNull] Func<CancellationToken, Task<TArg>> start, IEnumerable<Func<TArg, CancellationToken, Task<TArg>>>? funcs) =>
        new TaskRunner<TArg>(start, funcs).RunAsync();

    public static TaskRunner<TArg> StartWith(Func<CancellationToken, Task<TArg>> start) =>
        new(start);

    public static TaskRunner<TArg> StartWith(Func<Task<TArg>> start) =>
        StartWith(c => start());

    public static TaskRunner<TArg> StartWith(Func<TArg> start) =>
        StartWith(c => Task.FromResult(start()));

    public static TaskRunner<TArg> StartWith(TArg state) =>
        StartWith(c => Task.FromResult(state));

    public TaskRunner<TArg> OnEnded(Action<Result<TArg?>>? action) =>
        this.Fluent(this._onEnded = action);

    public async Task<Result<TArg?>> RunAsync(CancellationToken token = default)
    {
        this._isRunning = true;
        TArg? state = default;
        Result<TArg?> result = default!;
        try
        {
            state = await this._start(token);
            foreach (var func in this._funcList.Compact())
            {
                if (token.IsCancellationRequested)
                {
                    throw new OperationCancelException();
                }

                state = await func(state, token);
            }
            result = Result<TArg?>.CreateSuccess(state);
        }
        catch (Exception ex)
        {
            result = Result<TArg?>.CreateFailure(ex, state);
        }
        finally
        {
            this._isRunning = false;
            this._onEnded?.Invoke(result);
        }
        return result;
    }

    public TaskRunner<TArg> Then([DisallowNull] Func<TArg, CancellationToken, Task<TArg>> func)
    {
        Check.If(this._isRunning, () => new CommonException());
        Check.IfArgumentNotNull(func);
        this._funcList.Add(func);
        return this;
    }

    public TaskRunner<TArg> Then(Func<TArg, Task<TArg>> func) =>
        this.Then(new Func<TArg, CancellationToken, Task<TArg>>((x, _) => func(x)));

    public TaskRunner<TArg> Then(Func<TArg, Task> func) =>
        this.Then(new Func<TArg, CancellationToken, Task<TArg>>(async (x, _) =>
        {
            await func(x);
            return x;
        }));

    public TaskRunner<TArg> Then(Func<Task> func) =>
        this.Then(new Func<TArg, CancellationToken, Task<TArg>>(async (x, _) =>
        {
            await func();
            return x;
        }));
}