using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.String;

public class StringTransformExtensionsTests
{
    [Theory]
    [InlineData("hello world", "Hello World")]
    [InlineData("HELLO WORLD", "Hello World")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void ToTitleCase_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.ToTitleCase().Should().Be(expected);
    }

    [Theory]
    [InlineData("hello world", "helloWorld")]
    [InlineData("HelloWorld", "helloWorld")]
    [InlineData("hello-world", "helloWorld")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void ToCamelCase_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.ToCamelCase().Should().Be(expected);
    }

    [Theory]
    [InlineData("hello world", "HelloWorld")]
    [InlineData("helloWorld", "HelloWorld")]
    [InlineData("hello-world", "HelloWorld")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void ToPascalCase_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.ToPascalCase().Should().Be(expected);
    }

    [Theory]
    [InlineData("HelloWorld", "hello_world")]
    [InlineData("helloWorld", "hello_world")]
    [InlineData("hello world", "hello_world")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void ToSnakeCase_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.ToSnakeCase().Should().Be(expected);
    }

    [Theory]
    [InlineData("HelloWorld", "hello-world")]
    [InlineData("helloWorld", "hello-world")]
    [InlineData("hello world", "hello-world")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void ToKebabCase_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.ToKebabCase().Should().Be(expected);
    }

    [Theory]
    [InlineData("ğüşıöç", "gusioc")]
    [InlineData("ĞÜŞİÖÇ", "GUSIOC")]
    [InlineData("café", "cafe")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void RemoveDiacritics_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.RemoveDiacritics().Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello", "SGVsbG8=")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void ToBase64_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.ToBase64().Should().Be(expected);
    }

    [Theory]
    [InlineData("SGVsbG8=", "Hello")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void FromBase64_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.FromBase64().Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello", "48656C6C6F")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void ToHex_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.ToHex().Should().Be(expected);
    }

    [Theory]
    [InlineData("48656C6C6F", "Hello")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void FromHex_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.FromHex().Should().Be(expected);
    }

    [Theory]
    [InlineData("1234567890", 3, 4, '*', "123****890")]
    [InlineData("test", 0, 2, 'X', "XXst")]
    [InlineData("", 0, 2, '*', "")]
    [InlineData(null, 0, 2, '*', "")]
    public void Mask_ShouldReturnExpectedResult(string? input, int start, int length, char maskChar, string expected)
    {
        input.Mask(start, length, maskChar).Should().Be(expected);
    }

    [Fact]
    public void Format_ShouldInterpolateValues()
    {
        var template = "Hello {Name}, you are {Age} years old.";
        var data = new { Name = "John", Age = 30 };

        var result = template.Format(data);

        result.Should().Be("Hello John, you are 30 years old.");
    }
}
