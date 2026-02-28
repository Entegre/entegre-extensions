using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.Validation;

public class GuardTests
{
    [Fact]
    public void Null_ShouldThrowForNull()
    {
        string? value = null;

        var action = () => Guard.Against.Null(value);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Null_ShouldReturnValueForNonNull()
    {
        var value = "test";

        var result = Guard.Against.Null(value);

        result.Should().Be("test");
    }

    [Fact]
    public void NullOrEmpty_ShouldThrowForNull()
    {
        string? value = null;

        var action = () => Guard.Against.NullOrEmpty(value);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void NullOrEmpty_ShouldThrowForEmpty()
    {
        var value = "";

        var action = () => Guard.Against.NullOrEmpty(value);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void NullOrWhiteSpace_ShouldThrowForWhitespace()
    {
        var value = "   ";

        var action = () => Guard.Against.NullOrWhiteSpace(value);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void OutOfRange_ShouldThrowForValueBelowMin()
    {
        var action = () => Guard.Against.OutOfRange(5, 10, 20);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void OutOfRange_ShouldThrowForValueAboveMax()
    {
        var action = () => Guard.Against.OutOfRange(25, 10, 20);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void OutOfRange_ShouldReturnValueInRange()
    {
        var result = Guard.Against.OutOfRange(15, 10, 20);

        result.Should().Be(15);
    }

    [Fact]
    public void Zero_ShouldThrowForZero()
    {
        var action = () => Guard.Against.Zero(0);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Negative_ShouldThrowForNegative()
    {
        var action = () => Guard.Against.Negative(-1);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void NegativeOrZero_ShouldThrowForZero()
    {
        var action = () => Guard.Against.NegativeOrZero(0);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void NegativeOrZero_ShouldThrowForNegative()
    {
        var action = () => Guard.Against.NegativeOrZero(-1);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void InvalidEmail_ShouldThrowForInvalidEmail()
    {
        var action = () => Guard.Against.InvalidEmail("notanemail");

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void InvalidEmail_ShouldReturnValidEmail()
    {
        var result = Guard.Against.InvalidEmail("test@example.com");

        result.Should().Be("test@example.com");
    }

    [Fact]
    public void InvalidInput_ShouldThrowWhenPredicateIsTrue()
    {
        var action = () => Guard.Against.InvalidInput(5, x => x < 10, "Value must be >= 10");

        action.Should().Throw<ArgumentException>().WithMessage("*Value must be >= 10*");
    }

    [Fact]
    public void Default_ShouldThrowForDefaultValue()
    {
        var action = () => Guard.Against.Default(Guid.Empty);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void EmptyGuid_ShouldThrowForEmptyGuid()
    {
        var action = () => Guard.Against.EmptyGuid(Guid.Empty);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MaxLength_ShouldThrowForTooLong()
    {
        var action = () => Guard.Against.MaxLength("Hello World", 5);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MinLength_ShouldThrowForTooShort()
    {
        var action = () => Guard.Against.MinLength("Hi", 5);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Expression_ShouldThrowWhenConditionFails()
    {
        var action = () => Guard.Against.Expression(5, x => x > 10, "Value must be > 10");

        action.Should().Throw<ArgumentException>().WithMessage("*Value must be > 10*");
    }

    [Fact]
    public void NullOrEmpty_Collection_ShouldThrowForEmptyCollection()
    {
        var action = () => Guard.Against.NullOrEmpty(Array.Empty<int>());

        action.Should().Throw<ArgumentException>();
    }
}
