using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.Cryptography;

public class HashExtensionsTests
{
    [Fact]
    public void ToMD5_ShouldReturnConsistentHash()
    {
        var hash1 = "test".ToMD5();
        var hash2 = "test".ToMD5();

        hash1.Should().Be(hash2);
        hash1.Should().HaveLength(32);
    }

    [Fact]
    public void ToMD5_ShouldReturnKnownHash()
    {
        // Known MD5 hash for "hello"
        var hash = "hello".ToMD5();

        hash.Should().Be("5d41402abc4b2a76b9719d911017c592");
    }

    [Fact]
    public void ToSHA256_ShouldReturnConsistentHash()
    {
        var hash1 = "test".ToSHA256();
        var hash2 = "test".ToSHA256();

        hash1.Should().Be(hash2);
        hash1.Should().HaveLength(64);
    }

    [Fact]
    public void ToSHA512_ShouldReturnConsistentHash()
    {
        var hash1 = "test".ToSHA512();
        var hash2 = "test".ToSHA512();

        hash1.Should().Be(hash2);
        hash1.Should().HaveLength(128);
    }

    [Fact]
    public void ToHMACSHA256_ShouldReturnConsistentHash()
    {
        var hash1 = "test".ToHMACSHA256("secret");
        var hash2 = "test".ToHMACSHA256("secret");

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void ToHMACSHA256_DifferentKeys_ShouldReturnDifferentHashes()
    {
        var hash1 = "test".ToHMACSHA256("secret1");
        var hash2 = "test".ToHMACSHA256("secret2");

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void ToBCrypt_ShouldCreateVerifiableHash()
    {
        var password = "MyPassword123!";
        var hash = password.ToBCrypt();

        hash.Should().NotBeEmpty();
        hash.Should().StartWith("$2");
        password.VerifyBCrypt(hash).Should().BeTrue();
    }

    [Fact]
    public void VerifyBCrypt_WrongPassword_ShouldReturnFalse()
    {
        var hash = "MyPassword123!".ToBCrypt();

        "WrongPassword".VerifyBCrypt(hash).Should().BeFalse();
    }

    [Fact]
    public void VerifyBCrypt_InvalidHash_ShouldReturnFalse()
    {
        "test".VerifyBCrypt("invalidhash").Should().BeFalse();
    }

    [Fact]
    public void ToSHA256Bytes_ShouldReturn32Bytes()
    {
        var data = "test"u8.ToArray();
        var hash = data.ToSHA256Bytes();

        hash.Should().HaveCount(32);
    }

    [Fact]
    public void ToSHA512Bytes_ShouldReturn64Bytes()
    {
        var data = "test"u8.ToArray();
        var hash = data.ToSHA512Bytes();

        hash.Should().HaveCount(64);
    }
}
