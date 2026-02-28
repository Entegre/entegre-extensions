using System.Linq.Expressions;

namespace Entegre.Extensions;

/// <summary>
/// Represents a validation error.
/// </summary>
public record ValidationError(string PropertyName, string ErrorMessage);

/// <summary>
/// Represents the result of a validation.
/// </summary>
public class ValidationResult
{
    private readonly List<ValidationError> _errors = new();

    /// <summary>
    /// Gets whether the validation is valid.
    /// </summary>
    public bool IsValid => _errors.Count == 0;

    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    public IReadOnlyList<ValidationError> Errors => _errors.AsReadOnly();

    /// <summary>
    /// Adds an error to the result.
    /// </summary>
    internal void AddError(string propertyName, string message)
    {
        _errors.Add(new ValidationError(propertyName, message));
    }

    /// <summary>
    /// Gets all error messages.
    /// </summary>
    public IEnumerable<string> GetErrorMessages() => _errors.Select(e => e.ErrorMessage);

    /// <summary>
    /// Gets error messages for a specific property.
    /// </summary>
    public IEnumerable<string> GetErrorsFor(string propertyName) =>
        _errors.Where(e => e.PropertyName == propertyName).Select(e => e.ErrorMessage);

    /// <summary>
    /// Throws an exception if validation failed.
    /// </summary>
    public void ThrowIfInvalid()
    {
        if (!IsValid)
            throw new ValidationException(_errors);
    }
}

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationException : Exception
{
    public IReadOnlyList<ValidationError> Errors { get; }

    public ValidationException(IEnumerable<ValidationError> errors)
        : base("Validation failed: " + string.Join(", ", errors.Select(e => e.ErrorMessage)))
    {
        Errors = errors.ToList().AsReadOnly();
    }
}

/// <summary>
/// Provides extension methods for fluent validation.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Starts a validation builder for the object.
    /// </summary>
    public static ValidationBuilder<T> Validate<T>(this T obj)
    {
        return new ValidationBuilder<T>(obj);
    }
}

/// <summary>
/// Fluent validation builder.
/// </summary>
public class ValidationBuilder<T>
{
    private readonly T _obj;
    private readonly ValidationResult _result = new();

    public ValidationBuilder(T obj)
    {
        _obj = obj;
    }

    /// <summary>
    /// Validates that the property is not null.
    /// </summary>
    public ValidationBuilder<T> NotNull<TProp>(
        Expression<Func<T, TProp>> property,
        string? message = null)
    {
        var (name, value) = GetPropertyInfo(property);

        if (value is null)
        {
            _result.AddError(name, message ?? $"{name} is required.");
        }

        return this;
    }

    /// <summary>
    /// Validates that the string property is not null or empty.
    /// </summary>
    public ValidationBuilder<T> NotEmpty(
        Expression<Func<T, string?>> property,
        string? message = null)
    {
        var (name, value) = GetPropertyInfo(property);

        if (string.IsNullOrEmpty(value))
        {
            _result.AddError(name, message ?? $"{name} cannot be empty.");
        }

        return this;
    }

    /// <summary>
    /// Validates that the string property is not null, empty, or whitespace.
    /// </summary>
    public ValidationBuilder<T> NotWhiteSpace(
        Expression<Func<T, string?>> property,
        string? message = null)
    {
        var (name, value) = GetPropertyInfo(property);

        if (string.IsNullOrWhiteSpace(value))
        {
            _result.AddError(name, message ?? $"{name} cannot be empty or whitespace.");
        }

        return this;
    }

    /// <summary>
    /// Validates that the string property meets the minimum length.
    /// </summary>
    public ValidationBuilder<T> MinLength(
        Expression<Func<T, string?>> property,
        int minLength,
        string? message = null)
    {
        var (name, value) = GetPropertyInfo(property);

        if (value is not null && value.Length < minLength)
        {
            _result.AddError(name, message ?? $"{name} must be at least {minLength} characters.");
        }

        return this;
    }

    /// <summary>
    /// Validates that the string property does not exceed the maximum length.
    /// </summary>
    public ValidationBuilder<T> MaxLength(
        Expression<Func<T, string?>> property,
        int maxLength,
        string? message = null)
    {
        var (name, value) = GetPropertyInfo(property);

        if (value is not null && value.Length > maxLength)
        {
            _result.AddError(name, message ?? $"{name} cannot exceed {maxLength} characters.");
        }

        return this;
    }

    /// <summary>
    /// Validates that the property is a valid email.
    /// </summary>
    public ValidationBuilder<T> ValidEmail(
        Expression<Func<T, string?>> property,
        string? message = null)
    {
        var (name, value) = GetPropertyInfo(property);

        if (value is not null && !value.IsEmail())
        {
            _result.AddError(name, message ?? $"{name} is not a valid email address.");
        }

        return this;
    }

    /// <summary>
    /// Validates that the numeric property is within the specified range.
    /// </summary>
    public ValidationBuilder<T> InRange<TProp>(
        Expression<Func<T, TProp>> property,
        TProp min,
        TProp max,
        string? message = null) where TProp : IComparable<TProp>
    {
        var (name, value) = GetPropertyInfo(property);

        if (value is not null && (value.CompareTo(min) < 0 || value.CompareTo(max) > 0))
        {
            _result.AddError(name, message ?? $"{name} must be between {min} and {max}.");
        }

        return this;
    }

    /// <summary>
    /// Validates that the numeric property is greater than the specified value.
    /// </summary>
    public ValidationBuilder<T> GreaterThan<TProp>(
        Expression<Func<T, TProp>> property,
        TProp value,
        string? message = null) where TProp : IComparable<TProp>
    {
        var (name, propValue) = GetPropertyInfo(property);

        if (propValue is not null && propValue.CompareTo(value) <= 0)
        {
            _result.AddError(name, message ?? $"{name} must be greater than {value}.");
        }

        return this;
    }

    /// <summary>
    /// Validates that the numeric property is less than the specified value.
    /// </summary>
    public ValidationBuilder<T> LessThan<TProp>(
        Expression<Func<T, TProp>> property,
        TProp value,
        string? message = null) where TProp : IComparable<TProp>
    {
        var (name, propValue) = GetPropertyInfo(property);

        if (propValue is not null && propValue.CompareTo(value) >= 0)
        {
            _result.AddError(name, message ?? $"{name} must be less than {value}.");
        }

        return this;
    }

    /// <summary>
    /// Validates that the condition is true.
    /// </summary>
    public ValidationBuilder<T> Must(
        Func<T, bool> predicate,
        string message)
    {
        if (!predicate(_obj))
        {
            _result.AddError(string.Empty, message);
        }

        return this;
    }

    /// <summary>
    /// Validates the property with a custom predicate.
    /// </summary>
    public ValidationBuilder<T> Must<TProp>(
        Expression<Func<T, TProp>> property,
        Func<TProp, bool> predicate,
        string message)
    {
        var (name, value) = GetPropertyInfo(property);

        if (value is not null && !predicate(value))
        {
            _result.AddError(name, message);
        }

        return this;
    }

    /// <summary>
    /// Validates with a condition.
    /// </summary>
    public ValidationBuilder<T> When(
        bool condition,
        Action<ValidationBuilder<T>> rules)
    {
        if (condition)
        {
            rules(this);
        }

        return this;
    }

    /// <summary>
    /// Builds the validation result.
    /// </summary>
    public ValidationResult Build() => _result;

    private (string Name, TProp? Value) GetPropertyInfo<TProp>(Expression<Func<T, TProp>> property)
    {
        var memberExpression = property.Body as MemberExpression
            ?? (property.Body as UnaryExpression)?.Operand as MemberExpression;

        var name = memberExpression?.Member.Name ?? "Unknown";
        var value = property.Compile()(_obj);

        return (name, value);
    }
}
