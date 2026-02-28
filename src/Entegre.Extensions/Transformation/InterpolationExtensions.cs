using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Entegre.Extensions;

/// <summary>
/// Provides extension methods for string interpolation and templating.
/// </summary>
public static partial class InterpolationExtensions
{
    /// <summary>
    /// Interpolates named placeholders using an anonymous object.
    /// </summary>
    public static string Interpolate(this string template, object data)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(data);

        var properties = data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name, p => p.GetValue(data), StringComparer.OrdinalIgnoreCase);

        return InterpolateWith(template, properties);
    }

    /// <summary>
    /// Interpolates named placeholders using a dictionary.
    /// </summary>
    public static string InterpolateWith(this string template, IDictionary<string, object?> data)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(data);

        return PlaceholderRegex().Replace(template, match =>
        {
            var key = match.Groups[1].Value;

            // Handle nested properties (e.g., {Person.Name})
            var parts = key.Split('.');
            object? value = null;

            if (data.TryGetValue(parts[0], out value))
            {
                for (var i = 1; i < parts.Length && value is not null; i++)
                {
                    var prop = value.GetType().GetProperty(parts[i],
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    value = prop?.GetValue(value);
                }
            }

            return value?.ToString() ?? match.Value;
        });
    }

    /// <summary>
    /// Advanced templating with loops and conditions.
    /// </summary>
    public static string Template(this string template, object data)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(data);

        var result = template;

        // Process {{#each items}}...{{/each}} loops
        result = EachRegex().Replace(result, match =>
        {
            var collectionName = match.Groups[1].Value;
            var itemTemplate = match.Groups[2].Value;

            var prop = data.GetType().GetProperty(collectionName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (prop?.GetValue(data) is not System.Collections.IEnumerable collection)
                return string.Empty;

            var sb = new StringBuilder();
            var index = 0;

            foreach (var item in collection)
            {
                var itemResult = itemTemplate;

                // Replace {{@index}}
                itemResult = itemResult.Replace("{{@index}}", index.ToString());

                // Replace {{this}} for simple types
                if (item is string or IConvertible)
                {
                    itemResult = itemResult.Replace("{{this}}", item.ToString());
                }

                // Replace item properties
                itemResult = itemResult.Interpolate(item);

                sb.Append(itemResult);
                index++;
            }

            return sb.ToString();
        });

        // Process {{#if condition}}...{{/if}} conditionals
        result = IfRegex().Replace(result, match =>
        {
            var conditionName = match.Groups[1].Value;
            var content = match.Groups[2].Value;

            var prop = data.GetType().GetProperty(conditionName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            var value = prop?.GetValue(data);
            var isTruthy = IsTruthy(value);

            return isTruthy ? content.Interpolate(data) : string.Empty;
        });

        // Process {{#unless condition}}...{{/unless}} negative conditionals
        result = UnlessRegex().Replace(result, match =>
        {
            var conditionName = match.Groups[1].Value;
            var content = match.Groups[2].Value;

            var prop = data.GetType().GetProperty(conditionName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            var value = prop?.GetValue(data);
            var isTruthy = IsTruthy(value);

            return !isTruthy ? content.Interpolate(data) : string.Empty;
        });

        // Process simple placeholders
        result = result.Interpolate(data);

        return result;
    }

    /// <summary>
    /// Formats the value with culture-aware formatting.
    /// </summary>
    public static string Format(this object? value, string format, CultureInfo? culture = null)
    {
        if (value is null)
            return string.Empty;

        culture ??= CultureInfo.CurrentCulture;

        if (value is IFormattable formattable)
            return formattable.ToString(format, culture);

        return string.Format(culture, $"{{0:{format}}}", value);
    }

    /// <summary>
    /// Formats a number with culture-aware formatting.
    /// </summary>
    public static string FormatNumber(this decimal value, int decimals = 2, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        return value.ToString($"N{decimals}", culture);
    }

    /// <summary>
    /// Formats a currency value with culture-aware formatting.
    /// </summary>
    public static string FormatCurrency(this decimal value, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        return value.ToString("C", culture);
    }

    /// <summary>
    /// Formats a percentage value.
    /// </summary>
    public static string FormatPercent(this double value, int decimals = 2, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        return value.ToString($"P{decimals}", culture);
    }

    private static bool IsTruthy(object? value)
    {
        return value switch
        {
            null => false,
            bool b => b,
            string s => !string.IsNullOrEmpty(s),
            int i => i != 0,
            long l => l != 0,
            decimal d => d != 0,
            double dbl => dbl != 0,
            System.Collections.IEnumerable e => e.Cast<object>().Any(),
            _ => true
        };
    }

    [GeneratedRegex(@"\{(\w+(?:\.\w+)*)\}")]
    private static partial Regex PlaceholderRegex();

    [GeneratedRegex(@"\{\{#each\s+(\w+)\}\}(.*?)\{\{/each\}\}", RegexOptions.Singleline)]
    private static partial Regex EachRegex();

    [GeneratedRegex(@"\{\{#if\s+(\w+)\}\}(.*?)\{\{/if\}\}", RegexOptions.Singleline)]
    private static partial Regex IfRegex();

    [GeneratedRegex(@"\{\{#unless\s+(\w+)\}\}(.*?)\{\{/unless\}\}", RegexOptions.Singleline)]
    private static partial Regex UnlessRegex();
}
