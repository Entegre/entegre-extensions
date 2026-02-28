using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.Transformation;

public class JsonExtensionsTests
{
    [Fact]
    public void ToJson_ShouldSerializeObject()
    {
        var obj = new { Name = "John", Age = 30 };

        var json = obj.ToJson();

        json.Should().Contain("\"name\"");
        json.Should().Contain("\"John\"");
        json.Should().Contain("30");
    }

    [Fact]
    public void FromJson_ShouldDeserializeObject()
    {
        var json = "{\"name\":\"John\",\"age\":30}";

        var result = json.FromJson<TestPerson>();

        result.Should().NotBeNull();
        result!.Name.Should().Be("John");
        result.Age.Should().Be(30);
    }

    [Fact]
    public void ToJsonPretty_ShouldFormatWithIndentation()
    {
        var obj = new { Name = "John" };

        var json = obj.ToJsonPretty();

        json.Should().Contain("\n");
        json.Should().Contain("  ");
    }

    [Fact]
    public void TryFromJson_ValidJson_ShouldReturnTrue()
    {
        var json = "{\"name\":\"John\"}";

        var success = json.TryFromJson<TestPerson>(out var result);

        success.Should().BeTrue();
        result.Should().NotBeNull();
    }

    [Fact]
    public void TryFromJson_InvalidJson_ShouldReturnFalse()
    {
        var json = "invalid json";

        var success = json.TryFromJson<TestPerson>(out var result);

        success.Should().BeFalse();
        result.Should().BeNull();
    }

    [Fact]
    public void MergeJson_ShouldCombineObjects()
    {
        var json1 = "{\"name\":\"John\"}";
        var json2 = "{\"age\":30}";

        var merged = json1.MergeJson(json2);

        merged.Should().Contain("name");
        merged.Should().Contain("John");
        merged.Should().Contain("age");
        merged.Should().Contain("30");
    }

    [Fact]
    public void MergeJson_ShouldOverrideExistingProperties()
    {
        var json1 = "{\"name\":\"John\"}";
        var json2 = "{\"name\":\"Jane\"}";

        var merged = json1.MergeJson(json2);

        merged.Should().Contain("Jane");
        merged.Should().NotContain("John");
    }

    [Fact]
    public void GetJsonValue_ShouldGetNestedValue()
    {
        var json = "{\"person\":{\"name\":\"John\",\"address\":{\"city\":\"Istanbul\"}}}";

        var name = json.GetJsonValue<string>("person.name");
        var city = json.GetJsonValue<string>("person.address.city");

        name.Should().Be("John");
        city.Should().Be("Istanbul");
    }

    [Fact]
    public void GetJsonValue_ShouldHandleArrays()
    {
        var json = "{\"items\":[\"a\",\"b\",\"c\"]}";

        var value = json.GetJsonValue<string>("items[1]");

        value.Should().Be("b");
    }

    [Fact]
    public void IsValidJson_ShouldReturnTrueForValidJson()
    {
        "{\"name\":\"test\"}".IsValidJson().Should().BeTrue();
        "[1,2,3]".IsValidJson().Should().BeTrue();
    }

    [Fact]
    public void IsValidJson_ShouldReturnFalseForInvalidJson()
    {
        "not json".IsValidJson().Should().BeFalse();
        "{invalid}".IsValidJson().Should().BeFalse();
        "".IsValidJson().Should().BeFalse();
    }

    private class TestPerson
    {
        public string? Name { get; set; }
        public int Age { get; set; }
    }
}
