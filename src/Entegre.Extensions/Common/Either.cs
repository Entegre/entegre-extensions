namespace Entegre.Extensions;

/// <summary>
/// Represents a value that can be one of two types.
/// </summary>
public readonly struct Either<TLeft, TRight> : IEquatable<Either<TLeft, TRight>>
{
    private readonly TLeft _left;
    private readonly TRight _right;

    private Either(TLeft left, TRight right, bool isLeft)
    {
        _left = left;
        _right = right;
        IsLeft = isLeft;
    }

    /// <summary>
    /// Gets whether the value is the left type.
    /// </summary>
    public bool IsLeft { get; }

    /// <summary>
    /// Gets whether the value is the right type.
    /// </summary>
    public bool IsRight => !IsLeft;

    /// <summary>
    /// Gets the left value.
    /// </summary>
    public TLeft Left => IsLeft
        ? _left
        : throw new InvalidOperationException("Either is Right, not Left.");

    /// <summary>
    /// Gets the right value.
    /// </summary>
    public TRight Right => IsRight
        ? _right
        : throw new InvalidOperationException("Either is Left, not Right.");

    /// <summary>
    /// Creates an Either with a left value.
    /// </summary>
    public static Either<TLeft, TRight> FromLeft(TLeft left)
    {
        return new Either<TLeft, TRight>(left, default!, true);
    }

    /// <summary>
    /// Creates an Either with a right value.
    /// </summary>
    public static Either<TLeft, TRight> FromRight(TRight right)
    {
        return new Either<TLeft, TRight>(default!, right, false);
    }

    /// <summary>
    /// Maps the left value if present.
    /// </summary>
    public Either<TNewLeft, TRight> MapLeft<TNewLeft>(Func<TLeft, TNewLeft> mapper)
    {
        return IsLeft
            ? Either<TNewLeft, TRight>.FromLeft(mapper(Left))
            : Either<TNewLeft, TRight>.FromRight(Right);
    }

    /// <summary>
    /// Maps the right value if present.
    /// </summary>
    public Either<TLeft, TNewRight> MapRight<TNewRight>(Func<TRight, TNewRight> mapper)
    {
        return IsRight
            ? Either<TLeft, TNewRight>.FromRight(mapper(Right))
            : Either<TLeft, TNewRight>.FromLeft(Left);
    }

    /// <summary>
    /// Maps both values.
    /// </summary>
    public Either<TNewLeft, TNewRight> BiMap<TNewLeft, TNewRight>(
        Func<TLeft, TNewLeft> leftMapper,
        Func<TRight, TNewRight> rightMapper)
    {
        return IsLeft
            ? Either<TNewLeft, TNewRight>.FromLeft(leftMapper(Left))
            : Either<TNewLeft, TNewRight>.FromRight(rightMapper(Right));
    }

    /// <summary>
    /// Binds the left value to a new Either.
    /// </summary>
    public Either<TNewLeft, TRight> BindLeft<TNewLeft>(Func<TLeft, Either<TNewLeft, TRight>> binder)
    {
        return IsLeft
            ? binder(Left)
            : Either<TNewLeft, TRight>.FromRight(Right);
    }

    /// <summary>
    /// Binds the right value to a new Either.
    /// </summary>
    public Either<TLeft, TNewRight> BindRight<TNewRight>(Func<TRight, Either<TLeft, TNewRight>> binder)
    {
        return IsRight
            ? binder(Right)
            : Either<TLeft, TNewRight>.FromLeft(Left);
    }

    /// <summary>
    /// Matches the Either to a value.
    /// </summary>
    public TResult Match<TResult>(Func<TLeft, TResult> onLeft, Func<TRight, TResult> onRight)
    {
        return IsLeft ? onLeft(Left) : onRight(Right);
    }

    /// <summary>
    /// Executes an action based on the type.
    /// </summary>
    public void Match(Action<TLeft> onLeft, Action<TRight> onRight)
    {
        if (IsLeft)
            onLeft(Left);
        else
            onRight(Right);
    }

    /// <summary>
    /// Gets the left value or a default.
    /// </summary>
    public TLeft LeftOrDefault(TLeft defaultValue)
    {
        return IsLeft ? Left : defaultValue;
    }

    /// <summary>
    /// Gets the right value or a default.
    /// </summary>
    public TRight RightOrDefault(TRight defaultValue)
    {
        return IsRight ? Right : defaultValue;
    }

    /// <summary>
    /// Converts the left value to a Maybe.
    /// </summary>
    public Maybe<TLeft> LeftToMaybe()
    {
        return IsLeft ? Maybe<TLeft>.Some(Left) : Maybe<TLeft>.None;
    }

    /// <summary>
    /// Converts the right value to a Maybe.
    /// </summary>
    public Maybe<TRight> RightToMaybe()
    {
        return IsRight ? Maybe<TRight>.Some(Right) : Maybe<TRight>.None;
    }

    /// <summary>
    /// Swaps the left and right types.
    /// </summary>
    public Either<TRight, TLeft> Swap()
    {
        return IsLeft
            ? Either<TRight, TLeft>.FromRight(Left)
            : Either<TRight, TLeft>.FromLeft(Right);
    }

    public bool Equals(Either<TLeft, TRight> other)
    {
        if (IsLeft != other.IsLeft)
            return false;

        return IsLeft
            ? EqualityComparer<TLeft>.Default.Equals(Left, other.Left)
            : EqualityComparer<TRight>.Default.Equals(Right, other.Right);
    }

    public override bool Equals(object? obj)
    {
        return obj is Either<TLeft, TRight> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return IsLeft
            ? HashCode.Combine(true, Left)
            : HashCode.Combine(false, Right);
    }

    public static bool operator ==(Either<TLeft, TRight> left, Either<TLeft, TRight> right)
        => left.Equals(right);

    public static bool operator !=(Either<TLeft, TRight> left, Either<TLeft, TRight> right)
        => !left.Equals(right);

    public static implicit operator Either<TLeft, TRight>(TLeft left) => FromLeft(left);
    public static implicit operator Either<TLeft, TRight>(TRight right) => FromRight(right);

    public override string ToString()
    {
        return IsLeft ? $"Left({Left})" : $"Right({Right})";
    }
}

/// <summary>
/// Extension methods for Either types.
/// </summary>
public static class EitherExtensions
{
    /// <summary>
    /// Creates a left Either.
    /// </summary>
    public static Either<TLeft, TRight> ToLeft<TLeft, TRight>(this TLeft value)
    {
        return Either<TLeft, TRight>.FromLeft(value);
    }

    /// <summary>
    /// Creates a right Either.
    /// </summary>
    public static Either<TLeft, TRight> ToRight<TLeft, TRight>(this TRight value)
    {
        return Either<TLeft, TRight>.FromRight(value);
    }

    /// <summary>
    /// Gets all left values from a collection of Eithers.
    /// </summary>
    public static IEnumerable<TLeft> Lefts<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> eithers)
    {
        foreach (var either in eithers)
        {
            if (either.IsLeft)
                yield return either.Left;
        }
    }

    /// <summary>
    /// Gets all right values from a collection of Eithers.
    /// </summary>
    public static IEnumerable<TRight> Rights<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> eithers)
    {
        foreach (var either in eithers)
        {
            if (either.IsRight)
                yield return either.Right;
        }
    }

    /// <summary>
    /// Partitions a collection of Eithers into lefts and rights.
    /// </summary>
    public static (IReadOnlyList<TLeft> Lefts, IReadOnlyList<TRight> Rights) Partition<TLeft, TRight>(
        this IEnumerable<Either<TLeft, TRight>> eithers)
    {
        var lefts = new List<TLeft>();
        var rights = new List<TRight>();

        foreach (var either in eithers)
        {
            if (either.IsLeft)
                lefts.Add(either.Left);
            else
                rights.Add(either.Right);
        }

        return (lefts.AsReadOnly(), rights.AsReadOnly());
    }

    /// <summary>
    /// Traverses a collection and returns either the first error or all successes.
    /// </summary>
    public static Either<TLeft, IReadOnlyList<TRight>> Sequence<TLeft, TRight>(
        this IEnumerable<Either<TLeft, TRight>> eithers)
    {
        var rights = new List<TRight>();

        foreach (var either in eithers)
        {
            if (either.IsLeft)
                return Either<TLeft, IReadOnlyList<TRight>>.FromLeft(either.Left);

            rights.Add(either.Right);
        }

        return Either<TLeft, IReadOnlyList<TRight>>.FromRight(rights.AsReadOnly());
    }
}
