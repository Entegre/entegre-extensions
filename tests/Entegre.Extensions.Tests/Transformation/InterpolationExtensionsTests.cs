using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.Transformation;

public class InterpolationExtensionsTests
{
    [Fact]
    public void Interpolate_ShouldReplaceNamedPlaceholders()
    {
        var template = "Hello {Name}, you are {Age} years old!";
        var data = new { Name = "John", Age = 30 };

        var result = template.Interpolate(data);

        result.Should().Be("Hello John, you are 30 years old!");
    }

    [Fact]
    public void Interpolate_ShouldBeCaseInsensitive()
    {
        var template = "Hello {NAME}!";
        var data = new { Name = "John" };

        var result = template.Interpolate(data);

        result.Should().Be("Hello John!");
    }

    [Fact]
    public void InterpolateWith_ShouldUseDictionary()
    {
        var template = "Hello {Name}!";
        var data = new Dictionary<string, object?> { ["Name"] = "John" };

        var result = template.InterpolateWith(data);

        result.Should().Be("Hello John!");
    }

    [Fact]
    public void InterpolateWith_NestedProperties_ShouldResolve()
    {
        var template = "City: {Address.City}";
        var data = new Dictionary<string, object?>
        {
            ["Address"] = new { City = "Istanbul" }
        };

        var result = template.InterpolateWith(data);

        result.Should().Be("City: Istanbul");
    }

    [Fact]
    public void Template_Each_ShouldIterateCollection()
    {
        var template = "Items: {{#each Items}}{Name}, {{/each}}";
        var data = new
        {
            Items = new[]
            {
                new { Name = "A" },
                new { Name = "B" },
                new { Name = "C" }
            }
        };

        var result = template.Template(data);

        result.Should().Be("Items: A, B, C, ");
    }

    [Fact]
    public void Template_If_ShouldShowContentWhenTrue()
    {
        var template = "{{#if ShowMessage}}Hello!{{/if}}";

        var resultTrue = template.Template(new { ShowMessage = true });
        var resultFalse = template.Template(new { ShowMessage = false });

        resultTrue.Should().Be("Hello!");
        resultFalse.Should().BeEmpty();
    }

    [Fact]
    public void Template_Unless_ShouldShowContentWhenFalse()
    {
        var template = "{{#unless HideMessage}}Hello!{{/unless}}";

        var resultFalse = template.Template(new { HideMessage = false });
        var resultTrue = template.Template(new { HideMessage = true });

        resultFalse.Should().Be("Hello!");
        resultTrue.Should().BeEmpty();
    }

    [Fact]
    public void FormatNumber_ShouldFormatWithDecimals()
    {
        var value = 1234.5678m;

        var result = value.FormatNumber(2);

        result.Should().Contain("1");
        result.Should().Contain("234");
    }

    [Fact]
    public void FormatCurrency_ShouldFormatAsCurrency()
    {
        var value = 1234.56m;
        var culture = new System.Globalization.CultureInfo("en-US");

        var result = value.FormatCurrency(culture);

        result.Should().Contain("$");
        result.Should().Contain("1,234.56");
    }

    [Fact]
    public void FormatPercent_ShouldFormatAsPercentage()
    {
        var value = 0.1234;
        var culture = new System.Globalization.CultureInfo("en-US");

        var result = value.FormatPercent(2, culture);

        result.Should().Contain("12.34");
        result.Should().Contain("%");
    }
}
