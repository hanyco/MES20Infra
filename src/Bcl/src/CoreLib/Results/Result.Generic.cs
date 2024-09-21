using System.Diagnostics;
using System.Numerics;

using Library.DesignPatterns.Markers;
using Library.Interfaces;
using Library.Validations;

namespace Library.Results;

[DebuggerStepThrough, StackTraceHidden]
[Immutable]
[Fluent]
public class Result<TValue> : ResultBase, IResult<TValue>
    , IAdditionOperators<Result<TValue>, Result, Result<TValue>>
    , IAdditionOperators<Result<TValue>, Result<TValue>, Result<TValue>>
    , IEquatable<Result<TValue>>
    , ICombinable<Result<TValue>>
    , IFactory<Result<TValue>, Result<TValue>>
{
    public Result(
        TValue value,
        in bool? succeed = null,
        in string? message = null,
        in IEnumerable<Exception>? errors = null,
        in ResultBase? innerResult = null)
        : base(succeed, message, errors, innerResult) =>
        this.Value = value;

    public Result(ResultBase original, TValue value)
        : base(original)
        => this.Value = value;

    public TValue Value { get; init; }

    public static implicit operator Result(Result<TValue> result)
    {
        Check.MustBeArgumentNotNull(result);
        return new(result.IsSucceed, result.Message, result.Errors);
    }

    public static implicit operator Result<TValue>(TValue value) => new(value);

    public static implicit operator TValue(Result<TValue> result) => result.ArgumentNotNull().Value;

    public static Result<TValue>? New(Result<TValue>? arg) => arg == null ? default : new(arg, arg.Value);

    public static Result<TValue> operator +(Result<TValue> left, Result right)
    {
        Check.MustBeArgumentNotNull(left);
        return new Result<TValue>(left, left.Value) { InnerResult = right };
    }

    public static Result<TValue> operator +(Result<TValue> left, Result<TValue> right)
    {
        Check.MustBeArgumentNotNull(left);
        return new Result<TValue>(left, left.Value) { InnerResult = right };
    }

    public Result<TValue> Combine(Result obj) => this + obj;

    public Result<TValue> Combine(Result<TValue> obj) => this + obj;

    public bool Equals(Result<TValue>? other) => other is not null && this.GetHashCode() == other.GetHashCode();

    public override bool Equals(object? obj) => this.Equals(obj as Result<TValue>);

    public override int GetHashCode() => this.Value?.GetHashCode() ?? base.GetHashCode();

    public Result<TValue> SetMessage(string? message) => new(this) { Message = message };

    public Task<Result<TValue>> ToAsync() => Task.FromResult(this);

    public override string ToString() => this.IsFailure ? base.ToString() : this.Value?.ToString() ?? base.ToString();
}