using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.Collections;

public class CollectionExtensionsTests
{
    [Fact]
    public void IsNullOrEmpty_Collection_ShouldReturnTrueForNull()
    {
        List<int>? list = null;
        list.IsNullOrEmpty().Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_Collection_ShouldReturnTrueForEmpty()
    {
        var list = new List<int>();
        list.IsNullOrEmpty().Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_Collection_ShouldReturnFalseForNonEmpty()
    {
        var list = new List<int> { 1, 2, 3 };
        list.IsNullOrEmpty().Should().BeFalse();
    }

    [Fact]
    public void AddRange_ShouldAddMultipleItems()
    {
        var list = new List<int> { 1 };
        list.AddRange(new[] { 2, 3, 4 });

        list.Should().HaveCount(4);
        list.Should().ContainInOrder(1, 2, 3, 4);
    }

    [Fact]
    public void AddIf_ShouldAddWhenConditionIsTrue()
    {
        var list = new List<int> { 1 };

        list.AddIf(2, true).Should().BeTrue();
        list.Should().Contain(2);

        list.AddIf(3, false).Should().BeFalse();
        list.Should().NotContain(3);
    }

    [Fact]
    public void AddIf_WithPredicate_ShouldAddWhenPredicateIsTrue()
    {
        var list = new List<int> { 1 };

        list.AddIf(2, x => x > 0).Should().BeTrue();
        list.Should().Contain(2);

        list.AddIf(-1, x => x > 0).Should().BeFalse();
        list.Should().NotContain(-1);
    }

    [Fact]
    public void RemoveAll_ShouldRemoveMatchingItems()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };

        var removed = list.RemoveAll(x => x % 2 == 0);

        removed.Should().Be(2);
        list.Should().ContainInOrder(1, 3, 5);
    }

    [Fact]
    public void ReplaceAll_ShouldReplaceAllOccurrences()
    {
        var list = new List<int> { 1, 2, 2, 3, 2 };

        var replaced = list.ReplaceAll(2, 9);

        replaced.Should().Be(3);
        list.Should().ContainInOrder(1, 9, 9, 3, 9);
    }

    [Fact]
    public void ForEach_ShouldExecuteActionOnEachItem()
    {
        var list = new List<int> { 1, 2, 3 };
        var sum = 0;

        list.ForEach(x => sum += x);

        sum.Should().Be(6);
    }

    [Fact]
    public void ForEach_WithIndex_ShouldProvideIndex()
    {
        var list = new List<string> { "a", "b", "c" };
        var result = new List<string>();

        list.ForEach((item, index) => result.Add($"{index}:{item}"));

        result.Should().ContainInOrder("0:a", "1:b", "2:c");
    }

    [Fact]
    public void ToHashSet_ShouldConvertToHashSet()
    {
        var list = new List<int> { 1, 2, 2, 3, 3, 3 };

        var hashSet = list.ToHashSet();

        hashSet.Should().HaveCount(3);
        hashSet.Should().Contain(new[] { 1, 2, 3 });
    }
}
