namespace Entegre.Extensions;

/// <summary>
/// Represents the result of an operation that can succeed with a value or fail.
/// </summary>
public class Result<T> : Result
{
    private readonly T _value;

    internal Result(T value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value if successful.
    /// </summary>
    public T Value => IsSuccess
        ? _value
        : throw new InvalidOperationException("Cannot access value of a failed result.");

    /// <summary>
    /// Creates a success result with a value.
    /// </summary>
    public static Result<T> Success(T value) => new(value, true, Error.None);

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    public new static Result<T> Failure(Error error) => new(default!, false, error);

    /// <summary>
    /// Creates a failure result with a message.
    /// </summary>
    public new static Result<T> Failure(string message) => new(default!, false, Error.Failure(message));

    /// <summary>
    /// Maps the value to a new type if successful.
    /// </summary>
    public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        return IsSuccess
            ? Result<TNew>.Success(mapper(Value))
            : Result<TNew>.Failure(Error);
    }

    /// <summary>
    /// Maps the value to a new result if successful.
    /// </summary>
    public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> binder)
    {
        return IsSuccess
            ? binder(Value)
            : Result<TNew>.Failure(Error);
    }

    /// <summary>
    /// Gets the value or a default value if failed.
    /// </summary>
    public T GetValueOrDefault(T defaultValue = default!)
    {
        return IsSuccess ? Value : defaultValue;
    }

    /// <summary>
    /// Gets the value or throws the error as an exception.
    /// </summary>
    public T GetValueOrThrow()
    {
        if (IsFailure)
            throw new InvalidOperationException(Error.Message);

        return Value;
    }

    /// <summary>
    /// Gets the value or throws a custom exception.
    /// </summary>
    public T GetValueOrThrow(Func<Error, Exception> exceptionFactory)
    {
        if (IsFailure)
            throw exceptionFactory(Error);

        return Value;
    }

    /// <summary>
    /// Executes an action if successful.
    /// </summary>
    public Result<T> OnSuccess(Action<T> action)
    {
        if (IsSuccess)
            action(Value);

        return this;
    }

    /// <summary>
    /// Executes an action if failed.
    /// </summary>
    public Result<T> OnFailure(Action<Error> action)
    {
        if (IsFailure)
            action(Error);

        return this;
    }

    /// <summary>
    /// Matches the result to a value.
    /// </summary>
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value) : onFailure(Error);
    }

    /// <summary>
    /// Converts to a non-generic result.
    /// </summary>
    public Result ToResult()
    {
        return IsSuccess ? Result.Success() : Result.Failure(Error);
    }

    /// <summary>
    /// Implicit conversion from value to success result.
    /// </summary>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Implicit conversion from error to failure result.
    /// </summary>
    public static implicit operator Result<T>(Error error) => Failure(error);

    public override string ToString()
        => IsSuccess ? $"Success: {Value}" : $"Failure: {Error.Message}";
}
