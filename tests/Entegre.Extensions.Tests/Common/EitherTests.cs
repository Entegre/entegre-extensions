using FluentAssertions;
using Xunit;

namespace Entegre.Extensions.Tests.Common;

public class EitherTests
{
    [Fact]
    public void FromLeft_ShouldCreateLeftEither()
    {
        var either = Either<string, int>.FromLeft("error");

        either.IsLeft.Should().BeTrue();
        either.IsRight.Should().BeFalse();
        either.Left.Should().Be("error");
    }

    [Fact]
    public void FromRight_ShouldCreateRightEither()
    {
        var either = Either<string, int>.FromRight(42);

        either.IsRight.Should().BeTrue();
        either.IsLeft.Should().BeFalse();
        either.Right.Should().Be(42);
    }

    [Fact]
    public void Left_OnRight_ShouldThrow()
    {
        var either = Either<string, int>.FromRight(42);

        var action = () => _ = either.Left;

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Right_OnLeft_ShouldThrow()
    {
        var either = Either<string, int>.FromLeft("error");

        var action = () => _ = either.Right;

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void MapLeft_OnLeft_ShouldTransform()
    {
        var either = Either<string, int>.FromLeft("error");

        var mapped = either.MapLeft(s => s.ToUpper());

        mapped.IsLeft.Should().BeTrue();
        mapped.Left.Should().Be("ERROR");
    }

    [Fact]
    public void MapLeft_OnRight_ShouldNotChange()
    {
        var either = Either<string, int>.FromRight(42);

        var mapped = either.MapLeft(s => s.ToUpper());

        mapped.IsRight.Should().BeTrue();
        mapped.Right.Should().Be(42);
    }

    [Fact]
    public void MapRight_OnRight_ShouldTransform()
    {
        var either = Either<string, int>.FromRight(10);

        var mapped = either.MapRight(x => x * 2);

        mapped.IsRight.Should().BeTrue();
        mapped.Right.Should().Be(20);
    }

    [Fact]
    public void BiMap_ShouldTransformBoth()
    {
        var left = Either<string, int>.FromLeft("error");
        var right = Either<string, int>.FromRight(10);

        var mappedLeft = left.BiMap(s => s.Length, x => x * 2);
        var mappedRight = right.BiMap(s => s.Length, x => x * 2);

        mappedLeft.Left.Should().Be(5);
        mappedRight.Right.Should().Be(20);
    }

    [Fact]
    public void Match_ShouldCallCorrectFunction()
    {
        var left = Either<string, int>.FromLeft("error");
        var right = Either<string, int>.FromRight(42);

        left.Match(l => $"Left: {l}", r => $"Right: {r}").Should().Be("Left: error");
        right.Match(l => $"Left: {l}", r => $"Right: {r}").Should().Be("Right: 42");
    }

    [Fact]
    public void LeftOrDefault_ShouldReturnCorrectly()
    {
        var left = Either<string, int>.FromLeft("error");
        var right = Either<string, int>.FromRight(42);

        left.LeftOrDefault("default").Should().Be("error");
        right.LeftOrDefault("default").Should().Be("default");
    }

    [Fact]
    public void RightOrDefault_ShouldReturnCorrectly()
    {
        var left = Either<string, int>.FromLeft("error");
        var right = Either<string, int>.FromRight(42);

        left.RightOrDefault(0).Should().Be(0);
        right.RightOrDefault(0).Should().Be(42);
    }

    [Fact]
    public void LeftToMaybe_ShouldConvert()
    {
        var left = Either<string, int>.FromLeft("error");
        var right = Either<string, int>.FromRight(42);

        left.LeftToMaybe().HasValue.Should().BeTrue();
        right.LeftToMaybe().HasNoValue.Should().BeTrue();
    }

    [Fact]
    public void RightToMaybe_ShouldConvert()
    {
        var left = Either<string, int>.FromLeft("error");
        var right = Either<string, int>.FromRight(42);

        left.RightToMaybe().HasNoValue.Should().BeTrue();
        right.RightToMaybe().HasValue.Should().BeTrue();
    }

    [Fact]
    public void Swap_ShouldSwapSides()
    {
        var left = Either<string, int>.FromLeft("error");
        var right = Either<string, int>.FromRight(42);

        var swappedLeft = left.Swap();
        var swappedRight = right.Swap();

        swappedLeft.IsRight.Should().BeTrue();
        swappedLeft.Right.Should().Be("error");
        swappedRight.IsLeft.Should().BeTrue();
        swappedRight.Left.Should().Be(42);
    }

    [Fact]
    public void Equals_ShouldCompareCorrectly()
    {
        Either<string, int>.FromLeft("a").Should().Be(Either<string, int>.FromLeft("a"));
        Either<string, int>.FromRight(42).Should().Be(Either<string, int>.FromRight(42));
        Either<string, int>.FromLeft("a").Should().NotBe(Either<string, int>.FromRight(42));
    }

    [Fact]
    public void ImplicitConversion_ShouldWork()
    {
        Either<string, int> left = "error";
        Either<string, int> right = 42;

        left.IsLeft.Should().BeTrue();
        right.IsRight.Should().BeTrue();
    }

    [Fact]
    public void Lefts_ShouldExtractLeftValues()
    {
        var eithers = new[]
        {
            Either<string, int>.FromLeft("a"),
            Either<string, int>.FromRight(1),
            Either<string, int>.FromLeft("b"),
            Either<string, int>.FromRight(2)
        };

        var lefts = eithers.Lefts().ToList();

        lefts.Should().ContainInOrder("a", "b");
    }

    [Fact]
    public void Rights_ShouldExtractRightValues()
    {
        var eithers = new[]
        {
            Either<string, int>.FromLeft("a"),
            Either<string, int>.FromRight(1),
            Either<string, int>.FromLeft("b"),
            Either<string, int>.FromRight(2)
        };

        var rights = eithers.Rights().ToList();

        rights.Should().ContainInOrder(1, 2);
    }

    [Fact]
    public void Partition_ShouldSplitLeftsAndRights()
    {
        var eithers = new[]
        {
            Either<string, int>.FromLeft("a"),
            Either<string, int>.FromRight(1),
            Either<string, int>.FromLeft("b"),
            Either<string, int>.FromRight(2)
        };

        var (lefts, rights) = eithers.Partition();

        lefts.Should().ContainInOrder("a", "b");
        rights.Should().ContainInOrder(1, 2);
    }

    [Fact]
    public void Sequence_AllRights_ShouldReturnAllValues()
    {
        var eithers = new[]
        {
            Either<string, int>.FromRight(1),
            Either<string, int>.FromRight(2),
            Either<string, int>.FromRight(3)
        };

        var result = eithers.Sequence();

        result.IsRight.Should().BeTrue();
        result.Right.Should().ContainInOrder(1, 2, 3);
    }

    [Fact]
    public void Sequence_AnyLeft_ShouldReturnFirstLeft()
    {
        var eithers = new[]
        {
            Either<string, int>.FromRight(1),
            Either<string, int>.FromLeft("error"),
            Either<string, int>.FromRight(3)
        };

        var result = eithers.Sequence();

        result.IsLeft.Should().BeTrue();
        result.Left.Should().Be("error");
    }
}
