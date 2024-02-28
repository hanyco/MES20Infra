﻿using System.Diagnostics;
using System.Numerics;

using Library.DesignPatterns.Markers;
using Library.Interfaces;

namespace Library.Results;

[DebuggerStepThrough]
[StackTraceHidden]
[Immutable]
[Fluent]
public sealed class Result : ResultBase
    , IEmpty<Result>
    , IAdditionOperators<Result, ResultBase, Result>
    , IEquatable<Result>
    , ICombinable<Result>
    , ICombinable<ResultBase, Result>
{
    private static Result? _empty;
    private static Result? _failed;
    private static Result? _succeed;

    internal Result(
        in bool? succeed = null,
        in string? message = null,
        in IEnumerable<Exception>? errors = null,
        in IEnumerable<object>? extraData = null,
        in ResultBase? innerResult = null) : base(succeed, message, errors, extraData, innerResult)
    {
    }

    internal Result(ResultBase origin)
        : base(origin)
    {
    }

    public static Result Empty => _empty ??= NewEmpty();

    public static Result Failed => _failed ??= Fail();

    public static Result Succeed => _succeed ??= Success();

    public static Result Add(in Result left, in Result right)
        => left + right;

    public static explicit operator Result(bool b) =>
            b ? Succeed : Failed;

    
    public static Result Fail()
        => new(false, null, null, null, null);

    public static Result Fail(in string? message, in IEnumerable<Exception>? errors, in IEnumerable<object>? extraData)
        => new(false, message, errors: errors, extraData: extraData);

    public static Result Fail<TException>()
        where TException : Exception, new()
        => Fail(new TException());

    public static Result Fail(in string? message, in IEnumerable<Exception>? errors)
        => new(false, message, errors: errors);

    public static Result Fail(in string? message, in Exception error)
        => Fail(message: message, errors: [error]);

    public static Result Fail(in Exception error)
        => Fail(message: null, error);

    public static Result<TValue?> Fail<TValue>(in TValue? value,
        in string? message,
        in IEnumerable<Exception>? errors,
        in IEnumerable<object>? extraData)
        => new(value, false, message, errors, extraData);

    public static Result<TValue?> Fail<TValue>(in TValue? value,
        in string? message,
        in Exception error) =>
        new(value, false, message, EnumerableHelper.AsEnumerable(error), null);

    public static Result<TValue?> Fail<TValue>(in Exception error)
        => Fail<TValue>(default, null, error: error);

    public static Result<TValue?> Fail<TValue, TException>() where TException : Exception, new()
        => Fail<TValue>(new TException());

    public static Result<TValue?> Fail<TValue>(in Exception error, in TValue? value)
        => Fail(value, null, error: error)!;

    public static Result<TValue?> Fail<TValue>(in string message)
        => Fail<TValue>(default, message, null, null);

    public static Result<TValue?> Fail<TValue>(in string message, in TValue? value)
        => Fail(value, message, null, null);

    public static Result<TValue?> Fail<TValue>(in TValue? value = default)
        => Fail<TValue?>(value, null, null, null);
    
    public static Result<TValue> From<TValue>(in Result result, in TValue value)
        => new(result, value);

    public static Result NewEmpty() =>
        new();

    public static Result operator +(Result left, ResultBase right) =>
                                                                new(left) { InnerResult = right };

    public static Result Success(in object? status = null, in string? message = null) =>
        new(true, message);

    [return: NotNull]
    public static Result<TValue?> Success<TValue>(in TValue? value,
        in string? message = null,
        in IEnumerable<Exception>? errors = null,
        in IEnumerable<object>? extraData = null) =>
        new(value, true, message, errors, extraData);

    public Result Combine(Result obj) =>
            this + obj;

    public Result Combine(ResultBase obj) =>
        this + obj;

    public bool Equals(Result? other) =>
                other is not null && other == this;

    public override bool Equals(object? obj) =>
        this.Equals(obj as Result);

    public override int GetHashCode() =>
        base.GetHashCode();
}