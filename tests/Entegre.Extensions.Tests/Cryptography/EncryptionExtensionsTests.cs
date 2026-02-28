using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.Cryptography;

public class EncryptionExtensionsTests
{
    [Fact]
    public void EncryptDecryptAES_ShouldRoundTrip()
    {
        var original = "Hello, World! This is a secret message.";
        var password = "MySecretPassword123!";

        var encrypted = original.EncryptAES(password);
        var decrypted = encrypted.DecryptAES(password);

        decrypted.Should().Be(original);
    }

    [Fact]
    public void EncryptAES_ShouldProduceDifferentCiphertext()
    {
        var message = "Hello";
        var password = "secret";

        var encrypted1 = message.EncryptAES(password);
        var encrypted2 = message.EncryptAES(password);

        // Due to random IV, ciphertext should be different
        encrypted1.Should().NotBe(encrypted2);
    }

    [Fact]
    public void DecryptAES_WrongPassword_ShouldThrow()
    {
        var encrypted = "Hello".EncryptAES("correctpassword");

        var action = () => encrypted.DecryptAES("wrongpassword");

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void EncryptDecryptAES256_ShouldRoundTrip()
    {
        var original = "Hello, World!";
        var key = EncryptionExtensions.GenerateAESKey(256);
        var iv = EncryptionExtensions.GenerateIV();

        var encrypted = original.EncryptAES256(key, iv);
        var decrypted = encrypted.DecryptAES256(key, iv);

        decrypted.Should().Be(original);
    }

    [Fact]
    public void GenerateAESKey_ShouldGenerateCorrectSize()
    {
        var key128 = EncryptionExtensions.GenerateAESKey(128);
        var key192 = EncryptionExtensions.GenerateAESKey(192);
        var key256 = EncryptionExtensions.GenerateAESKey(256);

        key128.Should().HaveCount(16);
        key192.Should().HaveCount(24);
        key256.Should().HaveCount(32);
    }

    [Fact]
    public void GenerateIV_ShouldGenerate16Bytes()
    {
        var iv = EncryptionExtensions.GenerateIV();

        iv.Should().HaveCount(16);
    }

    [Fact]
    public void RSA_EncryptDecrypt_ShouldRoundTrip()
    {
        var (publicKey, privateKey) = EncryptionExtensions.GenerateRSAKeyPair(2048);
        var original = "Hello, RSA!";

        var encrypted = original.ToRSAEncrypt(publicKey);
        var decrypted = encrypted.FromRSADecrypt(privateKey);

        decrypted.Should().Be(original);
    }

    [Fact]
    public void GenerateRSAKeyPair_ShouldGenerateValidKeys()
    {
        var (publicKey, privateKey) = EncryptionExtensions.GenerateRSAKeyPair();

        publicKey.Should().Contain("<RSAKeyValue>");
        publicKey.Should().NotContain("<D>");  // Public key should not have private exponent

        privateKey.Should().Contain("<RSAKeyValue>");
        privateKey.Should().Contain("<D>");    // Private key should have private exponent
    }
}
