using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.Validation;

public class ValidationExtensionsTests
{
    [Fact]
    public void Validate_NotNull_ShouldFailForNull()
    {
        var person = new TestPerson { Name = null };

        var result = person.Validate()
            .NotNull(p => p.Name, "Name is required")
            .Build();

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Name is required");
    }

    [Fact]
    public void Validate_NotEmpty_ShouldFailForEmpty()
    {
        var person = new TestPerson { Name = "" };

        var result = person.Validate()
            .NotEmpty(p => p.Name, "Name cannot be empty")
            .Build();

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_MinLength_ShouldFailForShort()
    {
        var person = new TestPerson { Name = "Ab" };

        var result = person.Validate()
            .MinLength(p => p.Name, 3, "Name must be at least 3 characters")
            .Build();

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_MaxLength_ShouldFailForLong()
    {
        var person = new TestPerson { Name = "A very long name that exceeds the limit" };

        var result = person.Validate()
            .MaxLength(p => p.Name, 10, "Name too long")
            .Build();

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ValidEmail_ShouldFailForInvalid()
    {
        var person = new TestPerson { Email = "notanemail" };

        var result = person.Validate()
            .ValidEmail(p => p.Email, "Invalid email")
            .Build();

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_InRange_ShouldFailForOutOfRange()
    {
        var person = new TestPerson { Age = 150 };

        var result = person.Validate()
            .InRange(p => p.Age, 0, 120, "Age must be between 0 and 120")
            .Build();

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_GreaterThan_ShouldFailForLessThan()
    {
        var person = new TestPerson { Age = 10 };

        var result = person.Validate()
            .GreaterThan(p => p.Age, 18, "Must be over 18")
            .Build();

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Must_ShouldValidateWithPredicate()
    {
        var person = new TestPerson { Password = "abc", ConfirmPassword = "xyz" };

        var result = person.Validate()
            .Must(p => p.Password == p.ConfirmPassword, "Passwords must match")
            .Build();

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Passwords must match");
    }

    [Fact]
    public void Validate_When_ShouldConditionallyApplyRules()
    {
        var person = new TestPerson { Age = 15 };

        var result = person.Validate()
            .When(person.Age < 18, builder => builder
                .NotEmpty(p => p.ParentConsent, "Parent consent required for minors"))
            .Build();

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_MultipleRules_ShouldCollectAllErrors()
    {
        var person = new TestPerson { Name = "", Email = "invalid", Age = -1 };

        var result = person.Validate()
            .NotEmpty(p => p.Name, "Name required")
            .ValidEmail(p => p.Email, "Invalid email")
            .GreaterThan(p => p.Age, 0, "Age must be positive")
            .Build();

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(3);
    }

    [Fact]
    public void Validate_AllValid_ShouldPass()
    {
        var person = new TestPerson
        {
            Name = "John",
            Email = "john@example.com",
            Age = 30
        };

        var result = person.Validate()
            .NotEmpty(p => p.Name)
            .ValidEmail(p => p.Email)
            .InRange(p => p.Age, 0, 120)
            .Build();

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidationResult_ThrowIfInvalid_ShouldThrow()
    {
        var person = new TestPerson { Name = "" };

        var result = person.Validate()
            .NotEmpty(p => p.Name)
            .Build();

        var action = () => result.ThrowIfInvalid();

        action.Should().Throw<ValidationException>();
    }

    [Fact]
    public void ValidationResult_GetErrorMessages_ShouldReturnMessages()
    {
        var person = new TestPerson { Name = "", Email = "invalid" };

        var result = person.Validate()
            .NotEmpty(p => p.Name, "Name required")
            .ValidEmail(p => p.Email, "Email invalid")
            .Build();

        var messages = result.GetErrorMessages().ToList();

        messages.Should().HaveCount(2);
        messages.Should().Contain("Name required");
        messages.Should().Contain("Email invalid");
    }

    private class TestPerson
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int Age { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? ParentConsent { get; set; }
    }
}
