using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.String;

public class StringValidationExtensionsTests
{
    [Theory]
    [InlineData("12345", true)]
    [InlineData("123.45", false)]
    [InlineData("abc", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsNumeric_ShouldReturnExpectedResult(string? input, bool expected)
    {
        input.IsNumeric().Should().Be(expected);
    }

    [Theory]
    [InlineData("abc", true)]
    [InlineData("ABC", true)]
    [InlineData("abc123", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsAlpha_ShouldReturnExpectedResult(string? input, bool expected)
    {
        input.IsAlpha().Should().Be(expected);
    }

    [Theory]
    [InlineData("abc123", true)]
    [InlineData("ABC", true)]
    [InlineData("123", true)]
    [InlineData("abc-123", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsAlphanumeric_ShouldReturnExpectedResult(string? input, bool expected)
    {
        input.IsAlphanumeric().Should().Be(expected);
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("user.name+tag@example.co.uk", true)]
    [InlineData("invalid", false)]
    [InlineData("@example.com", false)]
    [InlineData("test@", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsEmail_ShouldReturnExpectedResult(string? input, bool expected)
    {
        input.IsEmail().Should().Be(expected);
    }

    [Theory]
    [InlineData("https://example.com", true)]
    [InlineData("http://localhost:8080/path", true)]
    [InlineData("ftp://example.com", false)]
    [InlineData("not a url", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsUrl_ShouldReturnExpectedResult(string? input, bool expected)
    {
        input.IsUrl().Should().Be(expected);
    }

    [Theory]
    [InlineData("+905551234567", true)]
    [InlineData("05551234567", true)]
    [InlineData("5551234567", true)]
    [InlineData("+1 555 123 4567", true)]
    [InlineData("123", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsPhoneNumber_ShouldReturnExpectedResult(string? input, bool expected)
    {
        input.IsPhoneNumber().Should().Be(expected);
    }

    [Theory]
    [InlineData("10000000146", true)]  // Valid TCKN
    [InlineData("12345678901", false)] // Invalid checksum
    [InlineData("01234567890", false)] // Starts with 0
    [InlineData("1234567890", false)]  // Too short
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsTCKN_ShouldReturnExpectedResult(string? input, bool expected)
    {
        input.IsTCKN().Should().Be(expected);
    }

    [Theory]
    [InlineData("GB82WEST12345698765432", true)]
    [InlineData("DE89370400440532013000", true)]
    [InlineData("TR330006100519786457841326", true)]
    [InlineData("INVALID", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsIBAN_ShouldReturnExpectedResult(string? input, bool expected)
    {
        input.IsIBAN().Should().Be(expected);
    }

    [Theory]
    [InlineData("4111111111111111", true)]  // Valid Visa
    [InlineData("5500000000000004", true)]  // Valid Mastercard
    [InlineData("1234567890123456", false)] // Invalid Luhn
    [InlineData("12345", false)]            // Too short
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsCreditCard_ShouldReturnExpectedResult(string? input, bool expected)
    {
        input.IsCreditCard().Should().Be(expected);
    }

    [Theory]
    [InlineData("{\"name\":\"test\"}", true)]
    [InlineData("[1,2,3]", true)]
    [InlineData("{invalid}", false)]
    [InlineData("not json", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsJson_ShouldReturnExpectedResult(string? input, bool expected)
    {
        input.IsJson().Should().Be(expected);
    }
}
