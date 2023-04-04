using System.Collections;

using Library.Validations;

namespace Library.Helpers;

public interface ICastable
{
    object? Value { get; }
}

public static class Caster
{
    public static T? As<T>([DisallowNull] this ICastable o) where T : class 
        => o.Value as T;

    public static T? To<T>([DisallowNull] this ICastable o)
        => (T?)o.Value;
    public static int ToInt([DisallowNull] this ICastable o)
        => Convert.ToInt32(o.Value);
    public static long ToLong([DisallowNull] this ICastable o)
        => Convert.ToInt64(o.Value);

    public static int CastToInt([DisallowNull] this ICastable o, int defaultValue)
    {
        if (!int.TryParse(Convert.ToString(o.Value), out var result))
        {
            result = defaultValue;
        }

        return result;
    }
    public static ICastable Cast(this object? obj) 
        => new Castable(obj);

    /// <summary>
    /// Works just like <code>as</code> C# keyword
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    [Obsolete($"Please use `{nameof(Cast)}().As<T>()`, instead.")]
    public static T? CastAs<T>(this object? obj)
        where T : class => obj as T;

    [Obsolete($"Please use `{nameof(Cast)}().To<T>()`, instead.")]
    public static T CastTo<T>(this object? obj)
        => (T)obj;

    [Obsolete($"Please use `{nameof(Cast)}().ToInt<T>()`, instead.")]
    public static int CastToInt(this object? obj, int defaultValue)
    {
        if (!int.TryParse(Convert.ToString(obj), out var result))
        {
            result = defaultValue;
        }

        return result;
    }

    /// <summary>
    /// Converts to long.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    //[Obsolete("No more is required.")]
    [Obsolete($"Please use {nameof(Cast)}(), instead.")]
    public static long CastToLong(this object obj)
        => Convert.ToInt64(obj);

    public static string? CastToString(in object? value, in string defaultValue = "")
        => (value ?? defaultValue)?.ToString();

    public static T? Match<T>(object obj) => obj is T result ? result : default;

    public static IEnumerable<T> OfType<T>(IEnumerable items)
        => items.OfType<T>();

    /// <summary>
    /// Converts the string representation of a number to an integer.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    public static int ToInt(this object obj)
        => Convert.ToInt32(obj);

    public static IEnumerable<T> TypeOf<T>(IEnumerable items)
    {
        foreach (var item in items)
        {
            if (TypeOf<T>(item) is { } result)
            {
                yield return result;
            }
        }
    }

    public static T? TypeOf<T>(object obj) => obj.GetType() == typeof(T) ? (T)obj : default;

    public static IEnumerable<T> WhereIs<T>(IEnumerable items)
    {
        foreach (var item in items)
        {
            if (item is T result)
            {
                yield return result;
            }
        }
    }
}

internal readonly record struct Castable(object? Value) : ICastable { }