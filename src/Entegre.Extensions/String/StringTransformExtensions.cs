using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Entegre.Extensions;

/// <summary>
/// Provides extension methods for string transformations.
/// </summary>
public static partial class StringTransformExtensions
{
    private static readonly Dictionary<char, char> TurkishCharMap = new()
    {
        { 'ğ', 'g' }, { 'Ğ', 'G' },
        { 'ü', 'u' }, { 'Ü', 'U' },
        { 'ş', 's' }, { 'Ş', 'S' },
        { 'ı', 'i' }, { 'İ', 'I' },
        { 'ö', 'o' }, { 'Ö', 'O' },
        { 'ç', 'c' }, { 'Ç', 'C' }
    };

    /// <summary>
    /// Converts the string to Title Case.
    /// </summary>
    public static string ToTitleCase(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var textInfo = CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(value.ToLower());
    }

    /// <summary>
    /// Converts the string to camelCase.
    /// </summary>
    public static string ToCamelCase(this string? value)
    {
        var pascal = value.ToPascalCase();
        if (string.IsNullOrEmpty(pascal))
            return string.Empty;

        return char.ToLowerInvariant(pascal[0]) + pascal[1..];
    }

    /// <summary>
    /// Converts the string to PascalCase.
    /// </summary>
    public static string ToPascalCase(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var words = SplitIntoWords(value);
        var sb = new StringBuilder();

        foreach (var word in words)
        {
            if (word.Length > 0)
            {
                sb.Append(char.ToUpperInvariant(word[0]));
                if (word.Length > 1)
                    sb.Append(word[1..].ToLowerInvariant());
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts the string to snake_case.
    /// </summary>
    public static string ToSnakeCase(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var words = SplitIntoWords(value);
        return string.Join("_", words.Select(w => w.ToLowerInvariant()));
    }

    /// <summary>
    /// Converts the string to kebab-case.
    /// </summary>
    public static string ToKebabCase(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var words = SplitIntoWords(value);
        return string.Join("-", words.Select(w => w.ToLowerInvariant()));
    }

    /// <summary>
    /// Removes diacritics (accents) from the string, including Turkish characters.
    /// </summary>
    public static string RemoveDiacritics(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var sb = new StringBuilder(value.Length);

        foreach (var c in value)
        {
            if (TurkishCharMap.TryGetValue(c, out var replacement))
            {
                sb.Append(replacement);
            }
            else
            {
                var normalized = c.ToString().Normalize(NormalizationForm.FormD);
                foreach (var nc in normalized)
                {
                    if (CharUnicodeInfo.GetUnicodeCategory(nc) != UnicodeCategory.NonSpacingMark)
                        sb.Append(nc);
                }
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Encodes the string to Base64.
    /// </summary>
    public static string ToBase64(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var bytes = Encoding.UTF8.GetBytes(value);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Decodes a Base64 encoded string.
    /// </summary>
    public static string FromBase64(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var bytes = Convert.FromBase64String(value);
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// Encodes the string to hexadecimal.
    /// </summary>
    public static string ToHex(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var bytes = Encoding.UTF8.GetBytes(value);
        return Convert.ToHexString(bytes);
    }

    /// <summary>
    /// Decodes a hexadecimal encoded string.
    /// </summary>
    public static string FromHex(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var bytes = Convert.FromHexString(value);
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// Masks a portion of the string with the specified character.
    /// </summary>
    public static string Mask(this string? value, int start, int length, char maskChar = '*')
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        if (start < 0 || start >= value.Length)
            return value;

        var actualLength = Math.Min(length, value.Length - start);
        var sb = new StringBuilder(value);

        for (var i = start; i < start + actualLength; i++)
        {
            sb[i] = maskChar;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Formats the string using named placeholders and an anonymous object.
    /// </summary>
    public static string Format(this string? template, object args)
    {
        if (string.IsNullOrEmpty(template))
            return string.Empty;

        var result = template;
        var properties = args.GetType().GetProperties();

        foreach (var prop in properties)
        {
            var placeholder = "{" + prop.Name + "}";
            var propValue = prop.GetValue(args)?.ToString() ?? string.Empty;
            result = result.Replace(placeholder, propValue, StringComparison.OrdinalIgnoreCase);
        }

        return result;
    }

    private static List<string> SplitIntoWords(string value)
    {
        // Handle camelCase and PascalCase
        var withSpaces = CamelCaseRegex().Replace(value, " $1");

        // Split by non-alphanumeric characters
        var words = WordSplitRegex().Split(withSpaces)
            .Where(w => !string.IsNullOrWhiteSpace(w))
            .ToList();

        return words;
    }

    [GeneratedRegex(@"([A-Z])")]
    private static partial Regex CamelCaseRegex();

    [GeneratedRegex(@"[^a-zA-Z0-9]+")]
    private static partial Regex WordSplitRegex();
}
