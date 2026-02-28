using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Entegre.Extensions;

/// <summary>
/// Provides guard clauses for defensive programming.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Entry point for guard clauses.
    /// </summary>
    public static IGuardClause Against => GuardClause.Instance;
}

/// <summary>
/// Interface for guard clauses.
/// </summary>
public interface IGuardClause { }

internal sealed class GuardClause : IGuardClause
{
    internal static readonly GuardClause Instance = new();
    private GuardClause() { }
}

/// <summary>
/// Guard clause extension methods.
/// </summary>
public static class GuardClauseExtensions
{
    /// <summary>
    /// Throws if the value is null.
    /// </summary>
    public static T Null<T>(
        this IGuardClause _,
        [NotNull] T? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value is null)
            throw new ArgumentNullException(parameterName);

        return value;
    }

    /// <summary>
    /// Throws if the string is null or empty.
    /// </summary>
    public static string NullOrEmpty(
        this IGuardClause _,
        [NotNull] string? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("Value cannot be null or empty.", parameterName);

        return value;
    }

    /// <summary>
    /// Throws if the string is null, empty, or whitespace.
    /// </summary>
    public static string NullOrWhiteSpace(
        this IGuardClause _,
        [NotNull] string? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null, empty, or whitespace.", parameterName);

        return value;
    }

    /// <summary>
    /// Throws if the collection is null or empty.
    /// </summary>
    public static IEnumerable<T> NullOrEmpty<T>(
        this IGuardClause _,
        [NotNull] IEnumerable<T>? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value is null || !value.Any())
            throw new ArgumentException("Collection cannot be null or empty.", parameterName);

        return value;
    }

    /// <summary>
    /// Throws if the value is outside the specified range.
    /// </summary>
    public static T OutOfRange<T>(
        this IGuardClause _,
        T value,
        T min,
        T max,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            throw new ArgumentOutOfRangeException(parameterName, value, $"Value must be between {min} and {max}.");

        return value;
    }

    /// <summary>
    /// Throws if the value is zero.
    /// </summary>
    public static T Zero<T>(
        this IGuardClause _,
        T value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null) where T : struct, IComparable<T>
    {
        if (value.CompareTo(default) == 0)
            throw new ArgumentException("Value cannot be zero.", parameterName);

        return value;
    }

    /// <summary>
    /// Throws if the value is negative.
    /// </summary>
    public static T Negative<T>(
        this IGuardClause _,
        T value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null) where T : struct, IComparable<T>
    {
        if (value.CompareTo(default) < 0)
            throw new ArgumentException("Value cannot be negative.", parameterName);

        return value;
    }

    /// <summary>
    /// Throws if the value is negative or zero.
    /// </summary>
    public static T NegativeOrZero<T>(
        this IGuardClause _,
        T value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null) where T : struct, IComparable<T>
    {
        if (value.CompareTo(default) <= 0)
            throw new ArgumentException("Value must be greater than zero.", parameterName);

        return value;
    }

    /// <summary>
    /// Throws if the email format is invalid.
    /// </summary>
    public static string InvalidEmail(
        this IGuardClause _,
        [NotNull] string? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        Guard.Against.NullOrWhiteSpace(value, parameterName);

        if (!value.IsEmail())
            throw new ArgumentException("Value is not a valid email address.", parameterName);

        return value;
    }

    /// <summary>
    /// Throws if the predicate returns true.
    /// </summary>
    public static T InvalidInput<T>(
        this IGuardClause _,
        T value,
        Func<T, bool> invalidPredicate,
        string message,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (invalidPredicate(value))
            throw new ArgumentException(message, parameterName);

        return value;
    }

    /// <summary>
    /// Throws if the value is the default value.
    /// </summary>
    public static T Default<T>(
        this IGuardClause _,
        T value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (EqualityComparer<T>.Default.Equals(value, default!))
            throw new ArgumentException("Value cannot be the default value.", parameterName);

        return value;
    }

    /// <summary>
    /// Throws if the GUID is empty.
    /// </summary>
    public static Guid EmptyGuid(
        this IGuardClause _,
        Guid value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Guid cannot be empty.", parameterName);

        return value;
    }

    /// <summary>
    /// Throws if the string exceeds the maximum length.
    /// </summary>
    public static string MaxLength(
        this IGuardClause _,
        string value,
        int maxLength,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        Guard.Against.Null(value, parameterName);

        if (value.Length > maxLength)
            throw new ArgumentException($"String cannot exceed {maxLength} characters.", parameterName);

        return value;
    }

    /// <summary>
    /// Throws if the string is shorter than the minimum length.
    /// </summary>
    public static string MinLength(
        this IGuardClause _,
        string value,
        int minLength,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        Guard.Against.Null(value, parameterName);

        if (value.Length < minLength)
            throw new ArgumentException($"String must be at least {minLength} characters.", parameterName);

        return value;
    }

    /// <summary>
    /// Throws with custom condition and message.
    /// </summary>
    public static T Expression<T>(
        this IGuardClause _,
        T value,
        Func<T, bool> condition,
        string message,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (!condition(value))
            throw new ArgumentException(message, parameterName);

        return value;
    }
}
