using System.Security.Cryptography;
using System.Text;

namespace Entegre.Extensions;

/// <summary>
/// Provides extension methods for cryptographic hashing.
/// </summary>
public static class HashExtensions
{
    /// <summary>
    /// Computes the MD5 hash of the string. Note: MD5 is not secure for cryptographic purposes.
    /// </summary>
    public static string ToMD5(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = MD5.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>
    /// Computes the SHA256 hash of the string.
    /// </summary>
    public static string ToSHA256(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>
    /// Computes the SHA512 hash of the string.
    /// </summary>
    public static string ToSHA512(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = SHA512.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>
    /// Computes the HMAC-SHA256 of the string using the specified key.
    /// </summary>
    public static string ToHMACSHA256(this string value, string key)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(key);

        var keyBytes = Encoding.UTF8.GetBytes(key);
        var valueBytes = Encoding.UTF8.GetBytes(value);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(valueBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>
    /// Computes the HMAC-SHA256 of the string using the specified key bytes.
    /// </summary>
    public static string ToHMACSHA256(this string value, byte[] key)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(key);

        var valueBytes = Encoding.UTF8.GetBytes(value);

        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(valueBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>
    /// Hashes the password using BCrypt.
    /// </summary>
    public static string ToBCrypt(this string password, int workFactor = 11)
    {
        ArgumentNullException.ThrowIfNull(password);
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor);
    }

    /// <summary>
    /// Verifies a password against a BCrypt hash.
    /// </summary>
    public static bool VerifyBCrypt(this string password, string hash)
    {
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(hash);

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Computes the SHA256 hash of the byte array.
    /// </summary>
    public static byte[] ToSHA256Bytes(this byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        return SHA256.HashData(data);
    }

    /// <summary>
    /// Computes the SHA512 hash of the byte array.
    /// </summary>
    public static byte[] ToSHA512Bytes(this byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        return SHA512.HashData(data);
    }
}
