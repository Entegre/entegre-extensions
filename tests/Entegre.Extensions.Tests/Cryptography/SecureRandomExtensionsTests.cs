using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.Cryptography;

public class SecureRandomExtensionsTests
{
    [Fact]
    public void GenerateSecureToken_ShouldGenerateCorrectLength()
    {
        var token16 = SecureRandomExtensions.GenerateSecureToken(16);
        var token32 = SecureRandomExtensions.GenerateSecureToken(32);

        token16.Should().HaveLength(16);
        token32.Should().HaveLength(32);
    }

    [Fact]
    public void GenerateSecureToken_ShouldGenerateUniqueTokens()
    {
        var tokens = Enumerable.Range(0, 100)
            .Select(_ => SecureRandomExtensions.GenerateSecureToken(32))
            .ToList();

        tokens.Distinct().Should().HaveCount(100);
    }

    [Fact]
    public void GenerateSecureTokenHex_ShouldBeHexadecimal()
    {
        var token = SecureRandomExtensions.GenerateSecureTokenHex(32);

        token.Should().HaveLength(32);
        token.Should().MatchRegex("^[0-9a-f]+$");
    }

    [Fact]
    public void GeneratePassword_ShouldMeetRequirements()
    {
        var password = SecureRandomExtensions.GeneratePassword(16);

        password.Should().HaveLength(16);
        password.Should().MatchRegex("[a-z]");  // Has lowercase
        password.Should().MatchRegex("[A-Z]");  // Has uppercase
        password.Should().MatchRegex("[0-9]");  // Has digit
        password.Should().MatchRegex("[!@#$%^&*()_+\\-=\\[\\]{}|;:,.<>?]");  // Has special
    }

    [Fact]
    public void GeneratePassword_LowercaseOnly_ShouldOnlyContainLowercase()
    {
        var password = SecureRandomExtensions.GeneratePassword(
            16, includeLowercase: true, includeUppercase: false,
            includeDigits: false, includeSpecial: false);

        password.Should().MatchRegex("^[a-z]+$");
    }

    [Fact]
    public void GenerateOTP_ShouldGenerateDigitsOnly()
    {
        var otp = SecureRandomExtensions.GenerateOTP(6);

        otp.Should().HaveLength(6);
        otp.Should().MatchRegex("^[0-9]+$");
    }

    [Fact]
    public void GenerateOTP_ShouldGenerateUniqueOTPs()
    {
        var otps = Enumerable.Range(0, 100)
            .Select(_ => SecureRandomExtensions.GenerateOTP(6))
            .ToList();

        // With 6 digits (1M possibilities), 100 OTPs should likely be unique
        otps.Distinct().Count().Should().BeGreaterThan(95);
    }

    [Fact]
    public void NewSecureGuid_ShouldGenerateValidGuid()
    {
        var guid = SecureRandomExtensions.NewSecureGuid();

        guid.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void NewSecureGuid_ShouldGenerateUniqueGuids()
    {
        var guids = Enumerable.Range(0, 100)
            .Select(_ => SecureRandomExtensions.NewSecureGuid())
            .ToList();

        guids.Distinct().Should().HaveCount(100);
    }

    [Fact]
    public void GetSecureRandomInt_ShouldBeInRange()
    {
        for (var i = 0; i < 100; i++)
        {
            var value = SecureRandomExtensions.GetSecureRandomInt(10, 20);
            value.Should().BeInRange(10, 19);
        }
    }

    [Fact]
    public void GetSecureRandomBytes_ShouldReturnCorrectLength()
    {
        var bytes = SecureRandomExtensions.GetSecureRandomBytes(32);

        bytes.Should().HaveCount(32);
    }
}
