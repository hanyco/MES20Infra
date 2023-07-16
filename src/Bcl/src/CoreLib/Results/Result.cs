﻿using System.Collections.Immutable;
using System.Diagnostics;

using Library.Interfaces;
using Library.Validations;

namespace Library.Results;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public abstract record ResultBase(in bool? Succeed = null,
                                  in object? Status = null,
                                  in string? Message = null,
                                  in IEnumerable<(object Id, object Error)>? Errors = null,
                                  in ImmutableDictionary<string, object>? ExtraData = null)
{
    /// <summary>
    /// Checks if the operation was successful by checking the Succeed flag, Status and Errors.
    /// </summary>
    public virtual bool IsSucceed => this.Succeed ?? ((this.Status is null or 0 or 200) && (!this.Errors?.Any() ?? true));

    /// <summary>
    /// Gets a value indicating whether the operation has failed.
    /// </summary>
    public virtual bool IsFailure => !this.IsSucceed;

    public void Deconstruct(out bool isSucceed, out string message)
        => (isSucceed, message) = (this.IsSucceed, this.Message?.ToString() ?? string.Empty);

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this instance.</param>
    /// <returns>A value that indicates the relative order of the objects being compared.</returns>
    public virtual bool Equals(ResultBase? other)
        => other is not null && this.Status == other.Status;

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>The hash code for the current instance.</returns>
    public override int GetHashCode()
        => HashCode.Combine(this.Status, this.Message);

    public static implicit operator bool(ResultBase result)
        => result.NotNull().IsSucceed;

    /// <summary>
    /// Generates a string representation of the result object.
    /// </summary>
    /// <returns>A string representation of the result object.</returns>
    public override string ToString()
    {
        var result = new StringBuilder().AppendLine($"IsSucceed: {this.IsSucceed}");
        if (!this.Message.IsNullOrEmpty())
        {
            _ = result.AppendLine(this.Message);
        }
        else if (this.Errors?.Count() == 1)
        {
            _ = result.AppendLine(this.Errors!.First().Error?.ToString() ?? "An error occurred.");
        }
        else if (this.Errors?.Any() ?? false)
        {
            foreach (var errorMessage in this.Errors.Select(x => x.Error?.ToString()).Compact())
            {
                _ = result.AppendLine($"- {errorMessage}");
            }
        }

        return result.ToString();
    }

    private string GetDebuggerDisplay()
        => this.ToString();

    /// <summary>
    /// Combines multiple ResultBase objects into a single ResultBase object.
    /// </summary>
    /// <param name="results">The ResultBase objects to combine.</param>
    /// <returns>A ResultBase object containing the combined results.</returns>
    protected static (bool? Succeed, object? Status, string? Message, IEnumerable<(object Id, object Error)>? Errors, ImmutableDictionary<string, object>? ExtraData) Combine(params ResultBase[] results)
    {
        bool? isSucceed = results.All(x => x.Succeed == null) ? null : results.All(x => x.IsSucceed);
        var status = results.LastOrDefault(x => x.Status is not null)?.Status;
        var message = results.LastOrDefault(x => !x.Message.IsNullOrEmpty())?.Message;
        var errors = results.SelectMany(x => EnumerableHelper.DefaultIfEmpty(x?.Errors));
        var statusBuffer = results.Where(x => x.Status is not null).Select(x => x.Status).ToList();
        if (statusBuffer.Count > 1)
        {
            errors = errors.AddRangeImmuted(statusBuffer.Select(x => ((object)null!, x!)));
            status = null;
        }
        var extraData = combineExtraData(results);

        return (isSucceed, status, message, errors, extraData.ToImmutableDictionary());

        static IEnumerable<KeyValuePair<string, object>> combineExtraData(ResultBase[] results)
        {
            var lastData = results
                .Where(x => x?.ExtraData is not null)
                .SelectMany(x => x.ExtraData!)
                .Where(item => !item.IsDefault() && !item.Key.IsNullOrEmpty() && item.Value is not null);
            return !lastData.Any()
                ? results.Select(x => new KeyValuePair<string, object>("Previous Rsult", x))
                : lastData;
        }
    }
}

public record Result(in bool? Succeed = null,
                     in object? Status = null,
                     in string? Message = null,
                     in IEnumerable<(object Id, object Error)>? Errors = null,
                     in ImmutableDictionary<string, object>? ExtraData = null)
    : ResultBase(Succeed, Status, Message, Errors, ExtraData)
    , IEmpty<Result>
    , IAdditionOperators<Result, Result, Result>
    , IEquatable<Result>
{
    private static Result? _empty;
    private static Result? _fail;
    private static Result? _success;

    /// <summary>
    /// Gets an empty result.
    /// </summary>
    /// <returns>An empty result.</returns>
    public static Result Empty => _empty ??= NewEmpty();

    /// <summary>
    /// Gets a Result object representing a failed operation.
    /// </summary>
    /// <returns>A Result object representing a failed operation.</returns>
    public static Result Failure => _fail ??= CreateFailure();

    /// <summary>
    /// Get a new instance of the Result class representing a successful operation.
    /// </summary>
    /// <returns>A new instance of the Result class representing a successful operation.</returns>
    public static Result Success => _success ??= CreateSuccess();

    /// <summary>
    /// Creates a new Result object with a failure status.
    /// </summary>
    /// <param name="status">Optional status object.</param>
    /// <param name="message">Optional message string.</param>
    /// <param name="errors">Optional enumerable of (Id, Error) tuples.</param>
    /// <param name="extraData">Optional extra data dictionary.</param>
    /// <returns>A new Result object with a failure status.</returns>
    public static Result CreateFailure(in object? status = null, in string? message = null, in IEnumerable<(object Id, object Error)>? errors = null, in ImmutableDictionary<string, object>? extraData = null)
        => new(false, status, message, Errors: errors, extraData);

    /// <summary>
    /// Creates a new Result object with a failure status and the specified message and errors.
    /// </summary>
    /// <param name="message">The message associated with the failure.</param>
    /// <param name="errors">The errors associated with the failure.</param>
    /// <returns>A new Result object with a failure status and the specified message and errors.</returns>
    public static Result CreateFailure(in string? message, in IEnumerable<(object Id, object Error)>? errors)
        => new(false, null, message, Errors: errors);

    /// <summary>
    /// Creates a new Result object with a success status.
    /// </summary>
    /// <param name="status">Optional status object.</param>
    /// <param name="message">Optional message string.</param>
    /// <returns>A new Result object with a success status.</returns>
    public static Result CreateSuccess(in object? status = null, in string? message = null)
        => new(true, status, message);

    /// <summary>
    /// Creates a new empty Result object.
    /// </summary>
    public static Result NewEmpty()
        => new();

    public static explicit operator Result(bool b)
        => b ? Success : Failure;

    public static Result operator +(Result left, Result right)
    {
        var total = Combine(left, right);
        var result = new Result(total.Succeed, total.Status, total.Message, total.Errors, total.ExtraData);
        return result;
    }

    /// <summary>
    /// Creates a new Result object with a failure status and the specified message and error.
    /// </summary>
    /// <param name="message">The message to be included in the Result object.</param>
    /// <param name="error">The Exception to be included in the Result object.</param>
    /// <returns>A new Result object with a failure status and the specified message and error.</returns>
    public static Result CreateFailure(in string message, in Exception error)
        => CreateFailure(error, message);

    /// <summary>
    /// Creates a failure result with the given exception and optional message.
    /// </summary>
    /// <param name="error">The exception to use for the failure result.</param>
    /// <returns>A failure result with the given exception.</returns>
    public static Result CreateFailure(Exception error)
        => CreateFailure(error, null);

    /// <summary>
    /// Combines multiple Result objects into a single Result object.
    /// </summary>
    /// <param name="results">The Result objects to combine.</param>
    /// <returns>A single Result object containing the combined data.</returns>
    public static Result Combine(params Result[] results)
    {
        var data = ResultBase.Combine(results);
        var result = new Result(data.Succeed, data.Status, data.Message, data.Errors, data.ExtraData);
        return result;
    }
}

public record Result<TValue>(in TValue Value,
    in bool? Succeed = null,
    in object? Status = null,
    in string? Message = null,
    in IEnumerable<(object Id, object Error)>? Errors = null,
    in ImmutableDictionary<string, object>? ExtraData = null)
    : ResultBase(Succeed, Status, Message, Errors, ExtraData)
    , IAdditionOperators<Result<TValue>, ResultBase, Result<TValue>>
    , IEquatable<Result<TValue>>
{
    private static Result<TValue?>? _failure;

    /// <summary>
    /// Gets a Result object representing a failed operation.
    /// </summary>
    /// <returns>A Result object representing a failed operation.</returns>
    public static Result<TValue?> Failure => _failure ??= CreateFailure();

    /// <summary>
    /// Creates a new Result object with the given value, succeed, status, message, errors, and extraData.
    /// </summary>
    public static Result<TValue> New(in TValue value,
        in bool? succeed = null,
        in object? status = null,
        in string? message = null,
        in IEnumerable<(object Id, object Error)>? errors = null,
        in ImmutableDictionary<string, object>? extraData = null)
        => new(value, succeed, status, message, errors, extraData);

    public Result<TNewValue?> WithValue<TNewValue>(TNewValue? newValue)
        => Result<TNewValue?>.New(newValue, this.Succeed, this.Status, this.Message, this.Errors, this.ExtraData);

    /// <summary>
    /// Creates a new Result with the given parameters and a success value of false.
    /// </summary>
    /// <param name="value">The value of the Result.</param>
    /// <param name="status">The status of the Result.</param>
    /// <param name="message">The message of the Result.</param>
    /// <param name="errors">The errors of the Result.</param>
    /// <param name="extraData">The extra data of the Result.</param>
    /// <returns>A new Result with the given parameters and a success value of false.</returns>
    public static Result<TValue?> CreateFailure(in TValue? value = default,
        in object? status = null,
        in string? message = null,
        in IEnumerable<(object Id, object Error)>? errors = null,
        in ImmutableDictionary<string, object>? extraData = null)
        => new(value, false, status, message, errors, extraData);

    /// <summary>
    /// Creates a new Result with the given value, status, message, and error.
    /// </summary>
    /// <param name="value">The value of the Result.</param>
    /// <param name="status">The status of the Result.</param>
    /// <param name="message">The message of the Result.</param>
    /// <param name="error">The error of the Result.</param>
    /// <returns>A new Result with the given value, status, message, and error.</returns>
    public static Result<TValue?> CreateFailure(in TValue? value,
        in object? status,
        in string? message,
        in (object Id, object Error) error)
        => new(value, false, status, message, EnumerableHelper.ToEnumerable(error), null);

    /// <summary>
    /// Creates a new successful Result with the given value, status, message, errors and extra data.
    /// </summary>
    /// <param name="value">The value of the Result.</param>
    /// <param name="status">The status of the Result.</param>
    /// <param name="message">The message of the Result.</param>
    /// <param name="errors">The errors of the Result.</param>
    /// <param name="extraData">The extra data of the Result.</param>
    /// <returns>A new successful Result.</returns>
    public static Result<TValue> CreateSuccess(in TValue value,
        in object? status = null,
        in string? message = null,
        in IEnumerable<(object Id, object Error)>? errors = null,
        in ImmutableDictionary<string, object>? extraData = null)
        => new(value, true, status, message, errors, extraData);

    public static implicit operator Result(Result<TValue> result)
        => new(result.Succeed, result.Status, result.Message, result.Errors, result.ExtraData);

    public static Result<TValue> operator +(Result<TValue> left, ResultBase right)
    {
        var total = Combine(left, right);
        var result = new Result<TValue>(left.Value, total.Succeed, total.Status, total.Message, total.Errors, total.ExtraData);
        return result;
    }

    public static implicit operator TValue(Result<TValue> result)
        => result.Value;

    /// <summary> Combines multiple Result<TValue> objects into a single Result<TValue> object.
    /// </summary> <param name="results">The Result<TValue> objects to combine.</param> <returns>A
    /// single Result<TValue> object containing the combined results.</returns>
    public static Result<TValue> Combine(IEnumerable<Result<TValue>> results, Func<TValue, TValue, TValue> add) =>
        Combine(add, results.ToArray());

    public static Result<TValue> Combine(Func<TValue, TValue, TValue> add, params Result<TValue>[] resultArray)
    {
        var data = ResultBase.Combine(resultArray);
        var valueArray = resultArray.Select(x => x.Value).ToArray();
        var value = valueArray[0];
        foreach (var v in valueArray.Skip(1))
        {
            value = add(value, v);
        }
        var result = new Result<TValue>(value, data.Succeed, data.Status, data.Message, data.Errors, data.ExtraData);
        return result;
    }

    public Result<TValue> Add(Result<TValue> item, Func<TValue, TValue, TValue> add) =>
        Result<TValue>.Combine(add, item);

    public void Deconstruct(out bool isSucceed, out TValue Value) =>
        (isSucceed, Value) = (this.IsSucceed, this.Value);

    /// <summary> Converts a Result<TValue> to a Result. </summary>
    public Result ToResult(in Result<TValue> result) =>
        result;
    public static Result<TValue> From(in Result result, in TValue value)
        => new(value, result.Succeed, result.Status, result.Message, result.Errors, result.ExtraData);

    /// <summary>
    /// Creates a failure result with the specified message, exception and value.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="ex">The exception.</param>
    /// <param name="value">The value.</param>
    /// <returns>A failure result.</returns>
    public static Result<TValue?> CreateFailure(in string message, in Exception ex, in TValue? value)
        => CreateFailure(value, ex, message);

    /// <summary>
    /// Creates a Result with a failure status and an Exception.
    /// </summary>
    /// <param name="error">The Exception to be stored in the Result.</param>
    /// <param name="value">The value to be stored in the Result.</param>
    /// <returns>A Result with a failure status and an Exception.</returns>
    public static Result<TValue?> CreateFailure(in Exception error, in TValue? value = default)
        => CreateFailure(value, error, null);

    public static Result<TValue?> CreateFailure(in string message, in TValue value)
        => CreateFailure(value, null, message);

    /// <summary>
    /// Creates a new instance of the <see cref="Result{TValue}"/> class with the specified value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <returns>
    /// A new instance of the <see cref="Result{TValue}"/> class with the specified value.
    /// </returns>
    public Result<TValue> WithValue(in TValue value)
        => this with { Value = value };

    /// <summary>
    /// Creates a new instance of the Result class with the specified errors.
    /// </summary>
    /// <param name="errors">The errors to add to the Result.</param>
    /// <returns>A new instance of the Result class with the specified errors.</returns>
    public Result<TValue> WithError(params (object Id, object Error)[] errors)
        => this with { Errors = errors };

    /// <summary>
    /// Converts the current Result object to an asynchronous Task.
    /// </summary>
    public Task<Result<TValue>> ToAsync()
        => Task.FromResult(this);

    /// <summary>
    /// Gets the value of the current instance.
    /// </summary>
    /// <returns>The value of the current instance.</returns>
    public TValue GetValue()
        => this.Value;
}