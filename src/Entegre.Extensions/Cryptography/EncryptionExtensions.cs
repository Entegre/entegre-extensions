using System.Security.Cryptography;
using System.Text;

namespace Entegre.Extensions;

/// <summary>
/// Provides extension methods for encryption and decryption.
/// </summary>
public static class EncryptionExtensions
{
    /// <summary>
    /// Encrypts the string using AES with a password-derived key.
    /// </summary>
    public static string EncryptAES(this string plainText, string password)
    {
        ArgumentNullException.ThrowIfNull(plainText);
        ArgumentNullException.ThrowIfNull(password);

        using var aes = Aes.Create();
        var key = DeriveKey(password, aes.KeySize / 8);
        aes.Key = key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        // Prepend IV to encrypted data
        var result = new byte[aes.IV.Length + encryptedBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// Decrypts the AES encrypted string using a password-derived key.
    /// </summary>
    public static string DecryptAES(this string cipherText, string password)
    {
        ArgumentNullException.ThrowIfNull(cipherText);
        ArgumentNullException.ThrowIfNull(password);

        var fullCipher = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        var key = DeriveKey(password, aes.KeySize / 8);
        aes.Key = key;

        // Extract IV from beginning of cipher
        var iv = new byte[aes.BlockSize / 8];
        var cipher = new byte[fullCipher.Length - iv.Length];

        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }

    /// <summary>
    /// Encrypts the string using AES-256 with explicit key and IV.
    /// </summary>
    public static string EncryptAES256(this string plainText, byte[] key, byte[] iv)
    {
        ArgumentNullException.ThrowIfNull(plainText);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(iv);

        if (key.Length != 32)
            throw new ArgumentException("Key must be 256 bits (32 bytes)", nameof(key));

        if (iv.Length != 16)
            throw new ArgumentException("IV must be 128 bits (16 bytes)", nameof(iv));

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        return Convert.ToBase64String(encryptedBytes);
    }

    /// <summary>
    /// Decrypts the AES-256 encrypted string using explicit key and IV.
    /// </summary>
    public static string DecryptAES256(this string cipherText, byte[] key, byte[] iv)
    {
        ArgumentNullException.ThrowIfNull(cipherText);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(iv);

        if (key.Length != 32)
            throw new ArgumentException("Key must be 256 bits (32 bytes)", nameof(key));

        if (iv.Length != 16)
            throw new ArgumentException("IV must be 128 bits (16 bytes)", nameof(iv));

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        var cipherBytes = Convert.FromBase64String(cipherText);
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }

    /// <summary>
    /// Generates a cryptographically secure AES key.
    /// </summary>
    public static byte[] GenerateAESKey(int keySize = 256)
    {
        if (keySize != 128 && keySize != 192 && keySize != 256)
            throw new ArgumentException("Key size must be 128, 192, or 256 bits", nameof(keySize));

        return RandomNumberGenerator.GetBytes(keySize / 8);
    }

    /// <summary>
    /// Generates a cryptographically secure initialization vector.
    /// </summary>
    public static byte[] GenerateIV()
    {
        return RandomNumberGenerator.GetBytes(16);
    }

    /// <summary>
    /// Encrypts data using RSA with a public key.
    /// </summary>
    public static byte[] ToRSAEncrypt(this byte[] data, string publicKeyXml)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(publicKeyXml);

        using var rsa = RSA.Create();
        rsa.FromXmlString(publicKeyXml);

        return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
    }

    /// <summary>
    /// Encrypts a string using RSA with a public key.
    /// </summary>
    public static string ToRSAEncrypt(this string plainText, string publicKeyXml)
    {
        ArgumentNullException.ThrowIfNull(plainText);

        var data = Encoding.UTF8.GetBytes(plainText);
        var encrypted = data.ToRSAEncrypt(publicKeyXml);
        return Convert.ToBase64String(encrypted);
    }

    /// <summary>
    /// Decrypts data using RSA with a private key.
    /// </summary>
    public static byte[] FromRSADecrypt(this byte[] data, string privateKeyXml)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(privateKeyXml);

        using var rsa = RSA.Create();
        rsa.FromXmlString(privateKeyXml);

        return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
    }

    /// <summary>
    /// Decrypts a string using RSA with a private key.
    /// </summary>
    public static string FromRSADecrypt(this string cipherText, string privateKeyXml)
    {
        ArgumentNullException.ThrowIfNull(cipherText);

        var data = Convert.FromBase64String(cipherText);
        var decrypted = data.FromRSADecrypt(privateKeyXml);
        return Encoding.UTF8.GetString(decrypted);
    }

    /// <summary>
    /// Generates an RSA key pair.
    /// </summary>
    public static (string PublicKey, string PrivateKey) GenerateRSAKeyPair(int keySizeInBits = 2048)
    {
        using var rsa = RSA.Create(keySizeInBits);
        return (rsa.ToXmlString(false), rsa.ToXmlString(true));
    }

    private static byte[] DeriveKey(string password, int keySize)
    {
        // Use a fixed salt for simplicity - in production, use a random salt stored with the data
        var salt = "EntegreSalt2024"u8.ToArray();
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(keySize);
    }
}
