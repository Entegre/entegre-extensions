namespace Entegre.Extensions;

/// <summary>
/// Represents an error with a code and message.
/// </summary>
public record Error(string Code, string Message)
{
    public static Error None => new(string.Empty, string.Empty);
    public static Error Null => new("Null", "A null value was provided.");
    public static Error NotFound => new("NotFound", "The requested resource was not found.");
    public static Error Unauthorized => new("Unauthorized", "Access denied.");
    public static Error Validation(string message) => new("Validation", message);
    public static Error Failure(string message) => new("Failure", message);
    public static Error Conflict(string message) => new("Conflict", message);
}

/// <summary>
/// Represents the result of an operation that can succeed or fail.
/// </summary>
public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Success result cannot have an error.");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Failure result must have an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Gets whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error if the operation failed.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Creates a success result.
    /// </summary>
    public static Result Success() => new(true, Error.None);

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>
    /// Creates a failure result with a message.
    /// </summary>
    public static Result Failure(string message) => new(false, Error.Failure(message));

    /// <summary>
    /// Creates a success result with a value.
    /// </summary>
    public static Result<T> Success<T>(T value) => new(value, true, Error.None);

    /// <summary>
    /// Creates a failure result with a value type.
    /// </summary>
    public static Result<T> Failure<T>(Error error) => new(default!, false, error);

    /// <summary>
    /// Creates a failure result with a message.
    /// </summary>
    public static Result<T> Failure<T>(string message) => new(default!, false, Error.Failure(message));

    /// <summary>
    /// Creates a result from a condition.
    /// </summary>
    public static Result Create(bool condition, Error error)
        => condition ? Success() : Failure(error);

    /// <summary>
    /// Creates a result from a value, failing if null.
    /// </summary>
    public static Result<T> Create<T>(T? value) where T : class
        => value is not null ? Success(value) : Failure<T>(Error.Null);

    /// <summary>
    /// Creates a result from a nullable value, failing if null.
    /// </summary>
    public static Result<T> Create<T>(T? value) where T : struct
        => value.HasValue ? Success(value.Value) : Failure<T>(Error.Null);

    /// <summary>
    /// Combines multiple results.
    /// </summary>
    public static Result Combine(params Result[] results)
    {
        foreach (var result in results)
        {
            if (result.IsFailure)
                return result;
        }

        return Success();
    }

    /// <summary>
    /// Combines multiple results and aggregates errors.
    /// </summary>
    public static Result CombineAll(params Result[] results)
    {
        var errors = results.Where(r => r.IsFailure).Select(r => r.Error).ToList();

        if (errors.Count == 0)
            return Success();

        var combinedMessage = string.Join("; ", errors.Select(e => e.Message));
        return Failure(new Error("Multiple", combinedMessage));
    }

    public override string ToString()
        => IsSuccess ? "Success" : $"Failure: {Error.Message}";
}
