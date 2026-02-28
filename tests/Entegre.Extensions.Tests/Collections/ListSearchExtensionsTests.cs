using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.Collections;

public class ListSearchExtensionsTests
{
    private readonly List<TestPerson> _people = new()
    {
        new TestPerson { Id = 1, Name = "John Smith", Email = "john@example.com" },
        new TestPerson { Id = 2, Name = "Jane Doe", Email = "jane@example.com" },
        new TestPerson { Id = 3, Name = "Bob Johnson", Email = "bob@test.com" }
    };

    [Fact]
    public void Search_ShouldFindMatchingItems()
    {
        var results = _people.Search("john", p => p.Name, p => p.Email).ToList();

        results.Should().HaveCount(2);
        results.Select(p => p.Name).Should().Contain("John Smith", "Bob Johnson");
    }

    [Fact]
    public void Search_ShouldReturnAllForEmptyQuery()
    {
        var results = _people.Search("", p => p.Name).ToList();

        results.Should().HaveCount(3);
    }

    [Fact]
    public void FuzzySearch_ShouldFindSimilarItems()
    {
        var results = _people.FuzzySearch("John Smit", 0.7, p => p.Name).ToList();

        results.Should().NotBeEmpty();
        results.Select(p => p.Name).Should().Contain("John Smith");
    }

    [Fact]
    public void WhereIf_ShouldApplyConditionWhenTrue()
    {
        var results = _people.WhereIf(true, p => p.Id > 1).ToList();

        results.Should().HaveCount(2);
    }

    [Fact]
    public void WhereIf_ShouldNotApplyConditionWhenFalse()
    {
        var results = _people.WhereIf(false, p => p.Id > 1).ToList();

        results.Should().HaveCount(3);
    }

    [Fact]
    public void ContainsAny_ShouldReturnTrueIfAnyMatch()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };

        numbers.ContainsAny(3, 6, 7).Should().BeTrue();
        numbers.ContainsAny(6, 7, 8).Should().BeFalse();
    }

    [Fact]
    public void ContainsAll_ShouldReturnTrueIfAllMatch()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };

        numbers.ContainsAll(1, 2, 3).Should().BeTrue();
        numbers.ContainsAll(1, 2, 6).Should().BeFalse();
    }

    [Fact]
    public void FindDuplicates_ShouldReturnDuplicateItems()
    {
        var numbers = new[] { 1, 2, 2, 3, 3, 3, 4 };

        var duplicates = numbers.FindDuplicates().ToList();

        duplicates.Should().Contain(new[] { 2, 3 });
        duplicates.Should().NotContain(1);
        duplicates.Should().NotContain(4);
    }

    [Fact]
    public void FindDuplicatesBy_ShouldReturnDuplicatesByKey()
    {
        var items = new[]
        {
            new { Id = 1, Name = "A" },
            new { Id = 2, Name = "B" },
            new { Id = 1, Name = "C" }
        };

        var duplicates = items.FindDuplicatesBy(x => x.Id).ToList();

        duplicates.Should().HaveCount(2);
    }

    [Fact]
    public void OrderByDynamic_ShouldOrderByPropertyName()
    {
        var ordered = _people.OrderByDynamic("Name").ToList();

        ordered[0].Name.Should().Be("Bob Johnson");
        ordered[1].Name.Should().Be("Jane Doe");
        ordered[2].Name.Should().Be("John Smith");
    }

    [Fact]
    public void OrderByDynamic_Descending_ShouldOrderDescending()
    {
        var ordered = _people.OrderByDynamic("Id", descending: true).ToList();

        ordered[0].Id.Should().Be(3);
        ordered[1].Id.Should().Be(2);
        ordered[2].Id.Should().Be(1);
    }

    private class TestPerson
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
