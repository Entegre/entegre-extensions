using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.Common;

public class MaybeTests
{
    [Fact]
    public void Some_ShouldHaveValue()
    {
        var maybe = Maybe<int>.Some(42);

        maybe.HasValue.Should().BeTrue();
        maybe.HasNoValue.Should().BeFalse();
        maybe.Value.Should().Be(42);
    }

    [Fact]
    public void None_ShouldNotHaveValue()
    {
        var maybe = Maybe<int>.None;

        maybe.HasNoValue.Should().BeTrue();
        maybe.HasValue.Should().BeFalse();
    }

    [Fact]
    public void Value_OnNone_ShouldThrow()
    {
        var maybe = Maybe<int>.None;

        var action = () => _ = maybe.Value;

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Some_WithNull_ShouldThrow()
    {
        var action = () => Maybe<string>.Some(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void From_WithValue_ShouldReturnSome()
    {
        var maybe = Maybe<string>.From("hello");

        maybe.HasValue.Should().BeTrue();
        maybe.Value.Should().Be("hello");
    }

    [Fact]
    public void From_WithNull_ShouldReturnNone()
    {
        var maybe = Maybe<string>.From(null);

        maybe.HasNoValue.Should().BeTrue();
    }

    [Fact]
    public void Map_OnSome_ShouldTransformValue()
    {
        var maybe = Maybe<int>.Some(10);

        var mapped = maybe.Map(x => x * 2);

        mapped.HasValue.Should().BeTrue();
        mapped.Value.Should().Be(20);
    }

    [Fact]
    public void Map_OnNone_ShouldReturnNone()
    {
        var maybe = Maybe<int>.None;

        var mapped = maybe.Map(x => x * 2);

        mapped.HasNoValue.Should().BeTrue();
    }

    [Fact]
    public void Bind_OnSome_ShouldChain()
    {
        var maybe = Maybe<int>.Some(10);

        var bound = maybe.Bind(x =>
            x > 0 ? Maybe<int>.Some(x * 2) : Maybe<int>.None);

        bound.HasValue.Should().BeTrue();
        bound.Value.Should().Be(20);
    }

    [Fact]
    public void GetValueOrDefault_OnSome_ShouldReturnValue()
    {
        var maybe = Maybe<int>.Some(42);

        maybe.GetValueOrDefault(0).Should().Be(42);
    }

    [Fact]
    public void GetValueOrDefault_OnNone_ShouldReturnDefault()
    {
        var maybe = Maybe<int>.None;

        maybe.GetValueOrDefault(99).Should().Be(99);
    }

    [Fact]
    public void GetValueOrThrow_OnNone_ShouldThrowWithMessage()
    {
        var maybe = Maybe<int>.None;

        var action = () => maybe.GetValueOrThrow("No value!");

        action.Should().Throw<InvalidOperationException>().WithMessage("No value!");
    }

    [Fact]
    public void OnSome_ShouldExecuteWhenHasValue()
    {
        var executed = false;
        var maybe = Maybe<int>.Some(42);

        maybe.OnSome(_ => executed = true);

        executed.Should().BeTrue();
    }

    [Fact]
    public void OnNone_ShouldExecuteWhenNoValue()
    {
        var executed = false;
        var maybe = Maybe<int>.None;

        maybe.OnNone(() => executed = true);

        executed.Should().BeTrue();
    }

    [Fact]
    public void Match_ShouldCallCorrectFunction()
    {
        var some = Maybe<int>.Some(42);
        var none = Maybe<int>.None;

        some.Match(v => $"Value: {v}", () => "None").Should().Be("Value: 42");
        none.Match(v => $"Value: {v}", () => "None").Should().Be("None");
    }

    [Fact]
    public void Where_ShouldFilterValue()
    {
        var maybe = Maybe<int>.Some(42);

        maybe.Where(x => x > 40).HasValue.Should().BeTrue();
        maybe.Where(x => x > 50).HasNoValue.Should().BeTrue();
    }

    [Fact]
    public void Or_ShouldReturnAlternativeWhenNone()
    {
        var none = Maybe<int>.None;
        var some = Maybe<int>.Some(42);

        none.Or(Maybe<int>.Some(99)).Value.Should().Be(99);
        some.Or(Maybe<int>.Some(99)).Value.Should().Be(42);
    }

    [Fact]
    public void Equals_ShouldCompareCorrectly()
    {
        Maybe<int>.Some(42).Should().Be(Maybe<int>.Some(42));
        Maybe<int>.None.Should().Be(Maybe<int>.None);
        Maybe<int>.Some(42).Should().NotBe(Maybe<int>.Some(99));
    }

    [Fact]
    public void ImplicitConversion_ShouldWork()
    {
        Maybe<string> maybe = "hello";

        maybe.HasValue.Should().BeTrue();
        maybe.Value.Should().Be("hello");
    }

    [Fact]
    public void ToMaybe_ReferenceType_ShouldConvert()
    {
        string? value = "hello";
        string? nullValue = null;

        value.ToMaybe().HasValue.Should().BeTrue();
        nullValue.ToMaybe().HasNoValue.Should().BeTrue();
    }

    [Fact]
    public void ToMaybe_ValueType_ShouldConvert()
    {
        int? value = 42;
        int? nullValue = null;

        value.ToMaybe().HasValue.Should().BeTrue();
        nullValue.ToMaybe().HasNoValue.Should().BeTrue();
    }

    [Fact]
    public void TryGetValue_ShouldReturnMaybe()
    {
        var dict = new Dictionary<string, int> { ["a"] = 1 };

        dict.TryGetValue("a").HasValue.Should().BeTrue();
        dict.TryGetValue("b").HasNoValue.Should().BeTrue();
    }

    [Fact]
    public void FirstOrNone_ShouldReturnMaybe()
    {
        var list = new List<int> { 1, 2, 3 };
        var emptyList = new List<int>();

        list.FirstOrNone().HasValue.Should().BeTrue();
        emptyList.FirstOrNone().HasNoValue.Should().BeTrue();
    }

    [Fact]
    public void WhereSome_ShouldFilterMaybes()
    {
        var maybes = new[]
        {
            Maybe<int>.Some(1),
            Maybe<int>.None,
            Maybe<int>.Some(2),
            Maybe<int>.None,
            Maybe<int>.Some(3)
        };

        var values = maybes.WhereSome().ToList();

        values.Should().ContainInOrder(1, 2, 3);
    }
}
