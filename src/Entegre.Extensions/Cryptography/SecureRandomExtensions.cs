using System.Security.Cryptography;
using System.Text;

namespace Entegre.Extensions;

/// <summary>
/// Provides methods for generating cryptographically secure random values.
/// </summary>
public static class SecureRandomExtensions
{
    private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
    private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string DigitChars = "0123456789";
    private const string SpecialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

    /// <summary>
    /// Generates a cryptographically secure random token.
    /// </summary>
    public static string GenerateSecureToken(int length = 32)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(length, 0);

        var bytes = RandomNumberGenerator.GetBytes(length);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=')[..length];
    }

    /// <summary>
    /// Generates a cryptographically secure random token as hex.
    /// </summary>
    public static string GenerateSecureTokenHex(int length = 32)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(length, 0);

        var byteCount = (length + 1) / 2;
        var bytes = RandomNumberGenerator.GetBytes(byteCount);
        return Convert.ToHexString(bytes).ToLowerInvariant()[..length];
    }

    /// <summary>
    /// Generates a cryptographically secure password.
    /// </summary>
    public static string GeneratePassword(
        int length = 16,
        bool includeLowercase = true,
        bool includeUppercase = true,
        bool includeDigits = true,
        bool includeSpecial = true)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(length, 0);

        var charPool = new StringBuilder();

        if (includeLowercase) charPool.Append(LowercaseChars);
        if (includeUppercase) charPool.Append(UppercaseChars);
        if (includeDigits) charPool.Append(DigitChars);
        if (includeSpecial) charPool.Append(SpecialChars);

        if (charPool.Length == 0)
            throw new ArgumentException("At least one character type must be included");

        var chars = charPool.ToString();
        var password = new char[length];

        // Ensure at least one character from each required category
        var index = 0;
        var requiredChars = new List<string>();

        if (includeLowercase) requiredChars.Add(LowercaseChars);
        if (includeUppercase) requiredChars.Add(UppercaseChars);
        if (includeDigits) requiredChars.Add(DigitChars);
        if (includeSpecial) requiredChars.Add(SpecialChars);

        foreach (var requiredSet in requiredChars)
        {
            if (index < length)
            {
                password[index++] = requiredSet[RandomNumberGenerator.GetInt32(requiredSet.Length)];
            }
        }

        // Fill remaining characters
        for (; index < length; index++)
        {
            password[index] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
        }

        // Shuffle the password
        for (var i = password.Length - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (password[i], password[j]) = (password[j], password[i]);
        }

        return new string(password);
    }

    /// <summary>
    /// Generates a one-time password (OTP).
    /// </summary>
    public static string GenerateOTP(int length = 6)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(length, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(length, 10);

        var otp = new char[length];
        for (var i = 0; i < length; i++)
        {
            otp[i] = DigitChars[RandomNumberGenerator.GetInt32(10)];
        }

        return new string(otp);
    }

    /// <summary>
    /// Generates a cryptographically secure GUID.
    /// </summary>
    public static Guid NewSecureGuid()
    {
        var bytes = RandomNumberGenerator.GetBytes(16);

        // Set version to 4 (random)
        bytes[7] = (byte)((bytes[7] & 0x0F) | 0x40);
        // Set variant to RFC 4122
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);

        return new Guid(bytes);
    }

    /// <summary>
    /// Generates a random integer within a range.
    /// </summary>
    public static int GetSecureRandomInt(int minValue, int maxValue)
    {
        return RandomNumberGenerator.GetInt32(minValue, maxValue);
    }

    /// <summary>
    /// Generates random bytes.
    /// </summary>
    public static byte[] GetSecureRandomBytes(int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(count, 0);
        return RandomNumberGenerator.GetBytes(count);
    }
}
