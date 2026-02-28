using System.Diagnostics.CodeAnalysis;

namespace Entegre.Extensions;

/// <summary>
/// Represents an optional value that may or may not exist.
/// </summary>
public readonly struct Maybe<T> : IEquatable<Maybe<T>>
{
    private readonly T _value;

    private Maybe(T value, bool hasValue)
    {
        _value = value;
        HasValue = hasValue;
    }

    /// <summary>
    /// Gets whether this instance has a value.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue { get; }

    /// <summary>
    /// Gets whether this instance has no value.
    /// </summary>
    public bool HasNoValue => !HasValue;

    /// <summary>
    /// Gets the value if present.
    /// </summary>
    public T Value => HasValue
        ? _value
        : throw new InvalidOperationException("Maybe has no value.");

    /// <summary>
    /// Creates a Maybe with a value.
    /// </summary>
    public static Maybe<T> Some(T value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value), "Cannot create Some with null value.");

        return new Maybe<T>(value, true);
    }

    /// <summary>
    /// Creates a Maybe with no value.
    /// </summary>
    public static Maybe<T> None => new(default!, false);

    /// <summary>
    /// Creates a Maybe from a nullable value.
    /// </summary>
    public static Maybe<T> From(T? value)
    {
        return value is null ? None : Some(value);
    }

    /// <summary>
    /// Maps the value to a new type if present.
    /// </summary>
    public Maybe<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        return HasValue
            ? Maybe<TResult>.Some(mapper(Value))
            : Maybe<TResult>.None;
    }

    /// <summary>
    /// Maps the value to a new Maybe if present.
    /// </summary>
    public Maybe<TResult> Bind<TResult>(Func<T, Maybe<TResult>> binder)
    {
        return HasValue
            ? binder(Value)
            : Maybe<TResult>.None;
    }

    /// <summary>
    /// Gets the value or a default value if not present.
    /// </summary>
    public T GetValueOrDefault(T defaultValue)
    {
        return HasValue ? Value : defaultValue;
    }

    /// <summary>
    /// Gets the value or a default value from a factory if not present.
    /// </summary>
    public T GetValueOrDefault(Func<T> defaultValueFactory)
    {
        return HasValue ? Value : defaultValueFactory();
    }

    /// <summary>
    /// Gets the value or throws an exception if not present.
    /// </summary>
    public T GetValueOrThrow(string message)
    {
        return HasValue
            ? Value
            : throw new InvalidOperationException(message);
    }

    /// <summary>
    /// Gets the value or throws a custom exception if not present.
    /// </summary>
    public T GetValueOrThrow(Func<Exception> exceptionFactory)
    {
        return HasValue
            ? Value
            : throw exceptionFactory();
    }

    /// <summary>
    /// Executes an action if the value is present.
    /// </summary>
    public Maybe<T> OnSome(Action<T> action)
    {
        if (HasValue)
            action(Value);

        return this;
    }

    /// <summary>
    /// Executes an action if the value is not present.
    /// </summary>
    public Maybe<T> OnNone(Action action)
    {
        if (!HasValue)
            action();

        return this;
    }

    /// <summary>
    /// Matches the Maybe to a value.
    /// </summary>
    public TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone)
    {
        return HasValue ? onSome(Value) : onNone();
    }

    /// <summary>
    /// Filters the value based on a predicate.
    /// </summary>
    public Maybe<T> Where(Func<T, bool> predicate)
    {
        return HasValue && predicate(Value) ? this : None;
    }

    /// <summary>
    /// Returns this Maybe or an alternative if no value.
    /// </summary>
    public Maybe<T> Or(Maybe<T> alternative)
    {
        return HasValue ? this : alternative;
    }

    /// <summary>
    /// Returns this Maybe or an alternative from a factory if no value.
    /// </summary>
    public Maybe<T> Or(Func<Maybe<T>> alternativeFactory)
    {
        return HasValue ? this : alternativeFactory();
    }

    public bool Equals(Maybe<T> other)
    {
        if (!HasValue && !other.HasValue)
            return true;

        if (!HasValue || !other.HasValue)
            return false;

        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    public override bool Equals(object? obj)
    {
        return obj is Maybe<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HasValue ? Value?.GetHashCode() ?? 0 : 0;
    }

    public static bool operator ==(Maybe<T> left, Maybe<T> right) => left.Equals(right);
    public static bool operator !=(Maybe<T> left, Maybe<T> right) => !left.Equals(right);

    public static implicit operator Maybe<T>(T value) => From(value);

    public override string ToString()
    {
        return HasValue ? $"Some({Value})" : "None";
    }
}

/// <summary>
/// Extension methods for Maybe types.
/// </summary>
public static class MaybeExtensions
{
    /// <summary>
    /// Converts a nullable value to a Maybe.
    /// </summary>
    public static Maybe<T> ToMaybe<T>(this T? value) where T : class
    {
        return value is null ? Maybe<T>.None : Maybe<T>.Some(value);
    }

    /// <summary>
    /// Converts a nullable struct to a Maybe.
    /// </summary>
    public static Maybe<T> ToMaybe<T>(this T? value) where T : struct
    {
        return value.HasValue ? Maybe<T>.Some(value.Value) : Maybe<T>.None;
    }

    /// <summary>
    /// Tries to get a value from a dictionary as a Maybe.
    /// </summary>
    public static Maybe<TValue> TryGetValue<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key)
    {
        return dictionary.TryGetValue(key, out var value)
            ? Maybe<TValue>.Some(value)
            : Maybe<TValue>.None;
    }

    /// <summary>
    /// Gets the first element as a Maybe.
    /// </summary>
    public static Maybe<T> FirstOrNone<T>(this IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            return Maybe<T>.Some(item);
        }

        return Maybe<T>.None;
    }

    /// <summary>
    /// Gets the first element matching a predicate as a Maybe.
    /// </summary>
    public static Maybe<T> FirstOrNone<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        foreach (var item in source)
        {
            if (predicate(item))
                return Maybe<T>.Some(item);
        }

        return Maybe<T>.None;
    }

    /// <summary>
    /// Gets the single element as a Maybe.
    /// </summary>
    public static Maybe<T> SingleOrNone<T>(this IEnumerable<T> source)
    {
        using var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
            return Maybe<T>.None;

        var item = enumerator.Current;

        if (enumerator.MoveNext())
            return Maybe<T>.None;

        return Maybe<T>.Some(item);
    }

    /// <summary>
    /// Flattens a nested Maybe.
    /// </summary>
    public static Maybe<T> Flatten<T>(this Maybe<Maybe<T>> maybe)
    {
        return maybe.HasValue ? maybe.Value : Maybe<T>.None;
    }

    /// <summary>
    /// Filters out None values from a collection.
    /// </summary>
    public static IEnumerable<T> WhereSome<T>(this IEnumerable<Maybe<T>> maybes)
    {
        foreach (var maybe in maybes)
        {
            if (maybe.HasValue)
                yield return maybe.Value;
        }
    }
}
