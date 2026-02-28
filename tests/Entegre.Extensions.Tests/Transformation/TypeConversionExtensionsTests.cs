using System.ComponentModel;
using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.Transformation;

public class TypeConversionExtensionsTests
{
    [Fact]
    public void To_ShouldConvertStringToInt()
    {
        var result = "42".To<int>();
        result.Should().Be(42);
    }

    [Fact]
    public void To_ShouldConvertIntToString()
    {
        var result = 42.To<string>();
        result.Should().Be("42");
    }

    [Fact]
    public void ToOrDefault_ShouldReturnDefaultOnFailure()
    {
        var result = "invalid".ToOrDefault(42);
        result.Should().Be(42);
    }

    [Fact]
    public void TryConvert_ShouldReturnTrueOnSuccess()
    {
        var success = "42".TryConvert<int>(out var result);

        success.Should().BeTrue();
        result.Should().Be(42);
    }

    [Fact]
    public void TryConvert_ShouldReturnFalseOnFailure()
    {
        var success = "invalid".TryConvert<int>(out var result);

        success.Should().BeFalse();
        result.Should().Be(default);
    }

    [Theory]
    [InlineData("42", 42)]
    [InlineData("0", 0)]
    [InlineData("-5", -5)]
    [InlineData("invalid", 0)]
    public void ToInt_ShouldConvertCorrectly(object input, int expected)
    {
        input.ToInt().Should().Be(expected);
    }

    [Theory]
    [InlineData("100", 100L)]
    [InlineData("9999999999", 9999999999L)]
    public void ToLong_ShouldConvertCorrectly(object input, long expected)
    {
        input.ToLong().Should().Be(expected);
    }

    [Fact]
    public void ToDecimal_ShouldConvertCorrectly()
    {
        "123.45".ToDecimal().Should().Be(123.45m);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("1", true)]
    [InlineData("0", false)]
    [InlineData("yes", true)]
    [InlineData("no", false)]
    [InlineData("on", true)]
    [InlineData("off", false)]
    [InlineData("enabled", true)]
    [InlineData("disabled", false)]
    [InlineData("invalid", false)]
    public void ToBool_ShouldConvertCorrectly(object input, bool expected)
    {
        input.ToBool().Should().Be(expected);
    }

    [Fact]
    public void ToGuid_ShouldConvertValidGuid()
    {
        var guidString = "550e8400-e29b-41d4-a716-446655440000";

        var result = guidString.ToGuid();

        result.Should().Be(Guid.Parse(guidString));
    }

    [Fact]
    public void ToGuid_ShouldReturnEmptyForInvalid()
    {
        var result = "invalid".ToGuid();

        result.Should().Be(Guid.Empty);
    }

    [Fact]
    public void ToEnum_ShouldConvertByName()
    {
        var result = "Second".ToEnum<TestEnum>();

        result.Should().Be(TestEnum.Second);
    }

    [Fact]
    public void ToEnum_ShouldBeCaseInsensitive()
    {
        var result = "SECOND".ToEnum<TestEnum>();

        result.Should().Be(TestEnum.Second);
    }

    [Fact]
    public void ToEnum_ShouldConvertByValue()
    {
        var result = "1".ToEnum<TestEnum>();

        result.Should().Be(TestEnum.Second);
    }

    [Fact]
    public void ToEnum_ShouldConvertByDescription()
    {
        var result = "Third Option".ToEnum<TestEnum>();

        result.Should().Be(TestEnum.Third);
    }

    [Fact]
    public void ToEnum_ShouldReturnDefaultForInvalid()
    {
        var result = "invalid".ToEnum(TestEnum.First);

        result.Should().Be(TestEnum.First);
    }

    [Fact]
    public void ToDateTime_ShouldConvertValidDate()
    {
        var result = "2024-06-15".ToDateTime();

        result.Year.Should().Be(2024);
        result.Month.Should().Be(6);
        result.Day.Should().Be(15);
    }

    [Fact]
    public void ToDateTime_ShouldReturnDefaultForInvalid()
    {
        var defaultDate = new System.DateTime(2000, 1, 1);

        var result = "invalid".ToDateTime(defaultDate);

        result.Should().Be(defaultDate);
    }

    private enum TestEnum
    {
        First = 0,
        Second = 1,
        [Description("Third Option")]
        Third = 2
    }
}
