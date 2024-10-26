using System.Collections.Immutable;
using System.Diagnostics;

using Library.DesignPatterns.Markers;
using Library.Validations;

namespace Library.Results;

[DebuggerStepThrough, StackTraceHidden]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
[Immutable]
public abstract class ResultBase(
    in bool? isSucceed = null,
    in string? message = null,
    in IEnumerable<Exception>? errors = null,
    ResultBase? innerResult = null) : IResult
{
    private readonly bool? _isSucceed = isSucceed;
    private ImmutableArray<Exception>? _errors = errors?.ToImmutableArray();
    
    protected ResultBase(ResultBase result)
        : this(result.ArgumentNotNull().IsSucceed, result.Message, result.Errors, result.InnerResult)
    {
    }

    [NotNull]
    public ImmutableArray<Exception> Errors
    {
        get => this._errors ??= ImmutableArray<Exception>.Empty;
        init => this._errors = value;
    }

    public Exception? Exception => this.Errors.FirstOrDefault();

    public ResultBase? InnerResult { get; init; } = innerResult;

    public virtual bool IsFailure => !this.IsSucceed;

    public virtual bool IsSucceed
    {
        get => this._isSucceed ?? !this.Errors.Any();
        init => this._isSucceed = value;
    }

    public string? Message { get => field ?? this.Exception?.GetBaseException().Message; init; } = message;

    public virtual bool Equals(ResultBase? other) =>
        other is not null && this.GetHashCode() == other.GetHashCode();

    public override int GetHashCode() =>
        HashCode.Combine(this.IsSucceed, this.Message);

    public override string ToString() =>
        !string.IsNullOrEmpty(this.Message) ? this.Message
        : this.Errors.FirstOrDefault()?.ToString() ?? $"IsSucceed: {this.IsSucceed}";

    private string GetDebuggerDisplay() => this.ToString();
}