﻿#nullable disable

using System.Diagnostics;

using Library.Results;

namespace Library.Validations;

[DebuggerStepThrough]
[StackTraceHidden]
public sealed class ValidationResultSet<TValue>(TValue value, CheckBehavior behavior, string valueName)
{
    internal readonly CheckBehavior Behavior = behavior;
    internal readonly List<(Func<TValue, bool> IsValid, Func<Exception> OnError)> RuleList = new();
    internal readonly string _valueName = valueName;

    public IEnumerable<(Func<TValue, bool> IsValid, Func<Exception> OnError)> Rules => 
        this.RuleList.ToEnumerable();

    public TValue Value { get; } = value;

    public static implicit operator Result<TValue>(ValidationResultSet<TValue> source) =>
        source.Build();

    public static implicit operator TValue(ValidationResultSet<TValue> source) =>
        source.Value;
}

/// <summary>
/// Enum to define the behavior when checking a condition.
/// </summary>
public enum CheckBehavior
{
    GatherAll,
    ReturnFirstFailure,
    ThrowOnFail,
}