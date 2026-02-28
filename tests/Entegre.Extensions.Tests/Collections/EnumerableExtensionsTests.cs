using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.Collections;

public class EnumerableExtensionsTests
{
    [Fact]
    public void Batch_ShouldSplitIntoCorrectSizes()
    {
        var items = Enumerable.Range(1, 10);

        var batches = items.Batch(3).ToList();

        batches.Should().HaveCount(4);
        batches[0].Should().HaveCount(3);
        batches[1].Should().HaveCount(3);
        batches[2].Should().HaveCount(3);
        batches[3].Should().HaveCount(1);
    }

    [Fact]
    public void WhereNotNull_ShouldFilterNullValues()
    {
        var items = new List<string?> { "a", null, "b", null, "c" };

        var result = items.WhereNotNull().ToList();

        result.Should().HaveCount(3);
        result.Should().ContainInOrder("a", "b", "c");
    }

    [Fact]
    public void DistinctBy_ShouldReturnDistinctByProperty()
    {
        var items = new[]
        {
            new { Id = 1, Name = "A" },
            new { Id = 1, Name = "B" },
            new { Id = 2, Name = "C" }
        };

        var result = items.DistinctBy(x => x.Id).ToList();

        result.Should().HaveCount(2);
    }

    [Fact]
    public void SafeFirst_ShouldReturnDefaultForEmpty()
    {
        var empty = Enumerable.Empty<int>();

        empty.SafeFirst().Should().Be(0);
    }

    [Fact]
    public void SafeFirst_ShouldReturnDefaultForNull()
    {
        IEnumerable<int>? nullList = null;

        nullList.SafeFirst().Should().Be(0);
    }

    [Fact]
    public void EmptyIfNull_ShouldReturnEmptyForNull()
    {
        IEnumerable<int>? nullList = null;

        var result = nullList.EmptyIfNull();

        result.Should().BeEmpty();
    }

    [Fact]
    public void Shuffle_ShouldRandomizeOrder()
    {
        var items = Enumerable.Range(1, 100).ToList();

        var shuffled = items.Shuffle().ToList();

        shuffled.Should().HaveCount(100);
        shuffled.Should().Contain(items);
        shuffled.Should().NotEqual(items); // Very unlikely to be in same order
    }

    [Fact]
    public void TakeRandom_ShouldTakeRandomItems()
    {
        var items = Enumerable.Range(1, 100);

        var random = items.TakeRandom(5).ToList();

        random.Should().HaveCount(5);
        random.Should().OnlyContain(x => x >= 1 && x <= 100);
    }

    [Fact]
    public void Flatten_ShouldFlattenNestedCollections()
    {
        var nested = new List<List<int>>
        {
            new() { 1, 2 },
            new() { 3, 4 },
            new() { 5 }
        };

        var flat = nested.Flatten().ToList();

        flat.Should().ContainInOrder(1, 2, 3, 4, 5);
    }

    [Fact]
    public void Index_ShouldAddIndexToElements()
    {
        var items = new[] { "a", "b", "c" };

        var indexed = items.Index().ToList();

        indexed.Should().HaveCount(3);
        indexed[0].Should().Be(("a", 0));
        indexed[1].Should().Be(("b", 1));
        indexed[2].Should().Be(("c", 2));
    }

    [Fact]
    public void Partition_ShouldSplitByPredicate()
    {
        var numbers = new[] { 1, 2, 3, 4, 5, 6 };

        var (even, odd) = numbers.Partition(x => x % 2 == 0);

        even.Should().ContainInOrder(2, 4, 6);
        odd.Should().ContainInOrder(1, 3, 5);
    }

    [Fact]
    public void MinByOrDefault_ShouldReturnMinElement()
    {
        var items = new[]
        {
            new { Name = "A", Value = 3 },
            new { Name = "B", Value = 1 },
            new { Name = "C", Value = 2 }
        };

        var min = items.MinByOrDefault(x => x.Value);

        min.Should().NotBeNull();
        min!.Name.Should().Be("B");
    }

    [Fact]
    public void MaxByOrDefault_ShouldReturnMaxElement()
    {
        var items = new[]
        {
            new { Name = "A", Value = 3 },
            new { Name = "B", Value = 1 },
            new { Name = "C", Value = 2 }
        };

        var max = items.MaxByOrDefault(x => x.Value);

        max.Should().NotBeNull();
        max!.Name.Should().Be("A");
    }

    [Fact]
    public void ToReadOnly_ShouldReturnReadOnlyCollection()
    {
        var items = new[] { 1, 2, 3 };

        var readOnly = items.ToReadOnly();

        readOnly.Should().BeAssignableTo<IReadOnlyCollection<int>>();
        readOnly.Should().HaveCount(3);
    }
}
