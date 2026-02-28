using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Entegre.Extensions;

/// <summary>
/// Provides extension methods for string manipulation.
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    /// Determines whether the string is null or empty.
    /// </summary>
    public static bool IsNullOrEmpty(this string? value) => string.IsNullOrEmpty(value);

    /// <summary>
    /// Determines whether the string is null, empty, or consists only of white-space characters.
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Converts a string to a URL-friendly slug with Turkish character support.
    /// </summary>
    public static string ToSlug(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var slug = value.RemoveDiacritics().ToLowerInvariant();
        slug = InvalidSlugCharsRegex().Replace(slug, "");
        slug = MultipleHyphensRegex().Replace(slug, "-");
        slug = slug.Trim('-');

        return slug;
    }

    /// <summary>
    /// Truncates the string to the specified maximum length.
    /// </summary>
    public static string Truncate(this string? value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value ?? string.Empty;

        if (maxLength <= suffix.Length)
            return suffix[..maxLength];

        return string.Concat(value.AsSpan(0, maxLength - suffix.Length), suffix);
    }

    /// <summary>
    /// Gets the leftmost characters of the string.
    /// </summary>
    public static string Left(this string? value, int length)
    {
        if (string.IsNullOrEmpty(value) || length <= 0)
            return string.Empty;

        return value.Length <= length ? value : value[..length];
    }

    /// <summary>
    /// Gets the rightmost characters of the string.
    /// </summary>
    public static string Right(this string? value, int length)
    {
        if (string.IsNullOrEmpty(value) || length <= 0)
            return string.Empty;

        return value.Length <= length ? value : value[^length..];
    }

    /// <summary>
    /// Reverses the string.
    /// </summary>
    public static string Reverse(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var chars = value.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    /// <summary>
    /// Repeats the string the specified number of times.
    /// </summary>
    public static string Repeat(this string? value, int count)
    {
        if (string.IsNullOrEmpty(value) || count <= 0)
            return string.Empty;

        if (count == 1)
            return value;

        return string.Concat(Enumerable.Repeat(value, count));
    }

    /// <summary>
    /// Determines whether the string contains the specified value, ignoring case.
    /// </summary>
    public static bool ContainsIgnoreCase(this string? value, string toCheck)
    {
        if (value is null)
            return false;

        return value.Contains(toCheck, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether two strings are equal, ignoring case.
    /// </summary>
    public static bool EqualsIgnoreCase(this string? value, string? other)
    {
        return string.Equals(value, other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Removes all whitespace characters from the string.
    /// </summary>
    public static string RemoveWhitespace(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return WhitespaceRegex().Replace(value, "");
    }

    /// <summary>
    /// Collapses multiple whitespace characters into a single space.
    /// </summary>
    public static string CollapseWhitespace(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return MultipleWhitespaceRegex().Replace(value.Trim(), " ");
    }

    /// <summary>
    /// Splits the string by newline characters.
    /// </summary>
    public static string[] SplitLines(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return [];

        return value.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
    }

    [GeneratedRegex(@"[^a-z0-9\s-]")]
    private static partial Regex InvalidSlugCharsRegex();

    [GeneratedRegex(@"[\s-]+")]
    private static partial Regex MultipleHyphensRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex MultipleWhitespaceRegex();
}
