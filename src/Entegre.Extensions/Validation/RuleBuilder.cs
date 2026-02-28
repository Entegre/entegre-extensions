using System.Linq.Expressions;

namespace Entegre.Extensions;

/// <summary>
/// Represents a validation rule.
/// </summary>
public interface IValidationRule<T>
{
    /// <summary>
    /// Validates the object and returns the errors.
    /// </summary>
    IEnumerable<ValidationError> Validate(T obj);
}

/// <summary>
/// Builder for creating validation rules.
/// </summary>
public class RuleBuilder<T>
{
    private readonly List<IValidationRule<T>> _rules = new();

    /// <summary>
    /// Adds a rule for a property.
    /// </summary>
    public PropertyRuleBuilder<T, TProp> RuleFor<TProp>(Expression<Func<T, TProp>> property)
    {
        var builder = new PropertyRuleBuilder<T, TProp>(this, property);
        return builder;
    }

    /// <summary>
    /// Adds a custom rule.
    /// </summary>
    public RuleBuilder<T> Custom(Func<T, IEnumerable<ValidationError>> rule)
    {
        _rules.Add(new CustomRule<T>(rule));
        return this;
    }

    /// <summary>
    /// Validates the object.
    /// </summary>
    public ValidationResult Validate(T obj)
    {
        var result = new ValidationResult();

        foreach (var rule in _rules)
        {
            foreach (var error in rule.Validate(obj))
            {
                result.AddError(error.PropertyName, error.ErrorMessage);
            }
        }

        return result;
    }

    internal void AddRule(IValidationRule<T> rule)
    {
        _rules.Add(rule);
    }
}

/// <summary>
/// Builder for property validation rules.
/// </summary>
public class PropertyRuleBuilder<T, TProp>
{
    private readonly RuleBuilder<T> _parent;
    private readonly Expression<Func<T, TProp>> _property;
    private readonly string _propertyName;
    private readonly List<(Func<TProp, bool> Predicate, string Message)> _conditions = new();

    internal PropertyRuleBuilder(RuleBuilder<T> parent, Expression<Func<T, TProp>> property)
    {
        _parent = parent;
        _property = property;

        var memberExpression = property.Body as MemberExpression
            ?? (property.Body as UnaryExpression)?.Operand as MemberExpression;

        _propertyName = memberExpression?.Member.Name ?? "Unknown";
    }

    /// <summary>
    /// Validates that the property is not null.
    /// </summary>
    public PropertyRuleBuilder<T, TProp> NotNull(string? message = null)
    {
        _conditions.Add((v => v is not null, message ?? $"{_propertyName} is required."));
        return this;
    }

    /// <summary>
    /// Validates with a custom predicate.
    /// </summary>
    public PropertyRuleBuilder<T, TProp> Must(Func<TProp, bool> predicate, string message)
    {
        _conditions.Add((predicate, message));
        return this;
    }

    /// <summary>
    /// Builds the rule and returns to the parent builder.
    /// </summary>
    public RuleBuilder<T> Build()
    {
        _parent.AddRule(new PropertyRule<T, TProp>(_property, _propertyName, _conditions));
        return _parent;
    }

    /// <summary>
    /// Continues with another property rule.
    /// </summary>
    public PropertyRuleBuilder<T, TNewProp> RuleFor<TNewProp>(Expression<Func<T, TNewProp>> property)
    {
        Build();
        return _parent.RuleFor(property);
    }
}

internal class PropertyRule<T, TProp> : IValidationRule<T>
{
    private readonly Func<T, TProp> _getter;
    private readonly string _propertyName;
    private readonly List<(Func<TProp, bool> Predicate, string Message)> _conditions;

    public PropertyRule(
        Expression<Func<T, TProp>> property,
        string propertyName,
        List<(Func<TProp, bool> Predicate, string Message)> conditions)
    {
        _getter = property.Compile();
        _propertyName = propertyName;
        _conditions = conditions;
    }

    public IEnumerable<ValidationError> Validate(T obj)
    {
        var value = _getter(obj);

        foreach (var (predicate, message) in _conditions)
        {
            if (!predicate(value))
            {
                yield return new ValidationError(_propertyName, message);
            }
        }
    }
}

internal class CustomRule<T> : IValidationRule<T>
{
    private readonly Func<T, IEnumerable<ValidationError>> _rule;

    public CustomRule(Func<T, IEnumerable<ValidationError>> rule)
    {
        _rule = rule;
    }

    public IEnumerable<ValidationError> Validate(T obj)
    {
        return _rule(obj);
    }
}

/// <summary>
/// Extension methods for creating validators.
/// </summary>
public static class RuleBuilderExtensions
{
    /// <summary>
    /// Creates a new rule builder for the type.
    /// </summary>
    public static RuleBuilder<T> CreateValidator<T>()
    {
        return new RuleBuilder<T>();
    }
}
