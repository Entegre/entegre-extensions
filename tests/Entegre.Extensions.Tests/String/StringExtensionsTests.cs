using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.String;

public class StringExtensionsTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData(" ", false)]
    [InlineData("test", false)]
    public void IsNullOrEmpty_ShouldReturnExpectedResult(string? input, bool expected)
    {
        input.IsNullOrEmpty().Should().Be(expected);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData(" ", true)]
    [InlineData("  \t\n", true)]
    [InlineData("test", false)]
    public void IsNullOrWhiteSpace_ShouldReturnExpectedResult(string? input, bool expected)
    {
        input.IsNullOrWhiteSpace().Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello World", "hello-world")]
    [InlineData("Türkçe Karakterler", "turkce-karakterler")]
    [InlineData("Test 123 Test", "test-123-test")]
    [InlineData("Multiple   Spaces", "multiple-spaces")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void ToSlug_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.ToSlug().Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello World", 5, "...", "He...")]
    [InlineData("Hi", 5, "...", "Hi")]
    [InlineData("Hello", 5, "...", "Hello")]
    [InlineData("", 5, "...", "")]
    [InlineData(null, 5, "...", "")]
    public void Truncate_ShouldReturnExpectedResult(string? input, int maxLength, string suffix, string expected)
    {
        input.Truncate(maxLength, suffix).Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello", 3, "Hel")]
    [InlineData("Hi", 5, "Hi")]
    [InlineData("", 3, "")]
    [InlineData(null, 3, "")]
    [InlineData("Test", 0, "")]
    public void Left_ShouldReturnExpectedResult(string? input, int length, string expected)
    {
        input.Left(length).Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello", 3, "llo")]
    [InlineData("Hi", 5, "Hi")]
    [InlineData("", 3, "")]
    [InlineData(null, 3, "")]
    [InlineData("Test", 0, "")]
    public void Right_ShouldReturnExpectedResult(string? input, int length, string expected)
    {
        input.Right(length).Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello", "olleH")]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("a", "a")]
    public void Reverse_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.Reverse().Should().Be(expected);
    }

    [Theory]
    [InlineData("ab", 3, "ababab")]
    [InlineData("test", 0, "")]
    [InlineData("", 5, "")]
    [InlineData(null, 5, "")]
    public void Repeat_ShouldReturnExpectedResult(string? input, int count, string expected)
    {
        input.Repeat(count).Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello World", "world", true)]
    [InlineData("Hello World", "WORLD", true)]
    [InlineData("Hello World", "xyz", false)]
    [InlineData(null, "test", false)]
    public void ContainsIgnoreCase_ShouldReturnExpectedResult(string? input, string value, bool expected)
    {
        input.ContainsIgnoreCase(value).Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello", "hello", true)]
    [InlineData("Hello", "HELLO", true)]
    [InlineData("Hello", "World", false)]
    [InlineData(null, null, true)]
    [InlineData("test", null, false)]
    public void EqualsIgnoreCase_ShouldReturnExpectedResult(string? input, string? value, bool expected)
    {
        input.EqualsIgnoreCase(value).Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello World", "HelloWorld")]
    [InlineData("  test  ", "test")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void RemoveWhitespace_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.RemoveWhitespace().Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello   World", "Hello World")]
    [InlineData("  test  ", "test")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void CollapseWhitespace_ShouldReturnExpectedResult(string? input, string expected)
    {
        input.CollapseWhitespace().Should().Be(expected);
    }

    [Fact]
    public void SplitLines_ShouldSplitByNewlines()
    {
        var input = "line1\nline2\r\nline3\rline4";
        var result = input.SplitLines();

        result.Should().HaveCount(4);
        result.Should().ContainInOrder("line1", "line2", "line3", "line4");
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData(null, 0)]
    public void SplitLines_ShouldHandleEmptyAndNull(string? input, int expectedCount)
    {
        input.SplitLines().Should().HaveCount(expectedCount);
    }
}
