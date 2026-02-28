using System.Text.Json;
using System.Text.RegularExpressions;

namespace Entegre.Extensions;

/// <summary>
/// Provides extension methods for string validation.
/// </summary>
public static partial class StringValidationExtensions
{
    /// <summary>
    /// Determines whether the string contains only numeric characters.
    /// </summary>
    public static bool IsNumeric(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        return value.All(char.IsDigit);
    }

    /// <summary>
    /// Determines whether the string contains only alphabetic characters.
    /// </summary>
    public static bool IsAlpha(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        return value.All(char.IsLetter);
    }

    /// <summary>
    /// Determines whether the string contains only alphanumeric characters.
    /// </summary>
    public static bool IsAlphanumeric(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        return value.All(char.IsLetterOrDigit);
    }

    /// <summary>
    /// Determines whether the string is a valid email address.
    /// </summary>
    public static bool IsEmail(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return EmailRegex().IsMatch(value);
    }

    /// <summary>
    /// Determines whether the string is a valid URL.
    /// </summary>
    public static bool IsUrl(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// Determines whether the string is a valid phone number (with Turkish format support).
    /// </summary>
    public static bool IsPhoneNumber(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var cleaned = PhoneCleanRegex().Replace(value, "");
        return PhoneRegex().IsMatch(cleaned);
    }

    /// <summary>
    /// Validates a Turkish Identification Number (TCKN).
    /// </summary>
    public static bool IsTCKN(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != 11)
            return false;

        if (!value.All(char.IsDigit))
            return false;

        if (value[0] == '0')
            return false;

        var digits = value.Select(c => c - '0').ToArray();

        // Algorithm validation
        var oddSum = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
        var evenSum = digits[1] + digits[3] + digits[5] + digits[7];

        var digit10 = ((oddSum * 7) - evenSum) % 10;
        if (digit10 < 0) digit10 += 10;

        if (digits[9] != digit10)
            return false;

        var sumFirst10 = digits.Take(10).Sum() % 10;
        return digits[10] == sumFirst10;
    }

    /// <summary>
    /// Validates an International Bank Account Number (IBAN).
    /// </summary>
    public static bool IsIBAN(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var iban = value.Replace(" ", "").Replace("-", "").ToUpperInvariant();

        if (iban.Length < 15 || iban.Length > 34)
            return false;

        if (!IbanRegex().IsMatch(iban))
            return false;

        // Move first 4 chars to end and convert letters to numbers
        var rearranged = iban[4..] + iban[..4];
        var numericIban = string.Concat(rearranged.Select(c =>
            char.IsLetter(c) ? (c - 'A' + 10).ToString() : c.ToString()));

        // Mod 97 check
        var remainder = 0;
        foreach (var c in numericIban)
        {
            remainder = (remainder * 10 + (c - '0')) % 97;
        }

        return remainder == 1;
    }

    /// <summary>
    /// Validates a credit card number using the Luhn algorithm.
    /// </summary>
    public static bool IsCreditCard(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var cleaned = value.Replace(" ", "").Replace("-", "");

        if (cleaned.Length < 13 || cleaned.Length > 19)
            return false;

        if (!cleaned.All(char.IsDigit))
            return false;

        // Luhn algorithm
        var sum = 0;
        var alternate = false;

        for (var i = cleaned.Length - 1; i >= 0; i--)
        {
            var digit = cleaned[i] - '0';

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }

    /// <summary>
    /// Determines whether the string is valid JSON.
    /// </summary>
    public static bool IsJson(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        value = value.Trim();
        if ((!value.StartsWith('{') || !value.EndsWith('}')) &&
            (!value.StartsWith('[') || !value.EndsWith(']')))
            return false;

        try
        {
            JsonDocument.Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"[\s\-\(\)\.]+")]
    private static partial Regex PhoneCleanRegex();

    [GeneratedRegex(@"^(\+?90|0)?[5][0-9]{9}$|^(\+?1)?[2-9]\d{9}$|^\+?[1-9]\d{6,14}$")]
    private static partial Regex PhoneRegex();

    [GeneratedRegex(@"^[A-Z]{2}[0-9A-Z]+$")]
    private static partial Regex IbanRegex();
}
