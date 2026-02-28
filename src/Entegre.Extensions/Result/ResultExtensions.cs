namespace Entegre.Extensions;

/// <summary>
/// Provides extension methods for Result types.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a value to a success result.
    /// </summary>
    public static Result<T> ToResult<T>(this T value)
    {
        return Result.Success(value);
    }

    /// <summary>
    /// Converts a nullable value to a result, failing if null.
    /// </summary>
    public static Result<T> ToResult<T>(this T? value, Error error) where T : class
    {
        return value is not null
            ? Result.Success(value)
            : Result.Failure<T>(error);
    }

    /// <summary>
    /// Converts a nullable struct to a result, failing if null.
    /// </summary>
    public static Result<T> ToResult<T>(this T? value, Error error) where T : struct
    {
        return value.HasValue
            ? Result.Success(value.Value)
            : Result.Failure<T>(error);
    }

    /// <summary>
    /// Ensures a condition is met, otherwise returns a failure.
    /// </summary>
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, Error error)
    {
        if (result.IsFailure)
            return result;

        return predicate(result.Value)
            ? result
            : Result<T>.Failure(error);
    }

    /// <summary>
    /// Taps into a successful result without modifying it.
    /// </summary>
    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
            action(result.Value);

        return result;
    }

    /// <summary>
    /// Combines two results into a tuple.
    /// </summary>
    public static Result<(T1, T2)> Combine<T1, T2>(this Result<T1> result1, Result<T2> result2)
    {
        if (result1.IsFailure)
            return Result<(T1, T2)>.Failure(result1.Error);

        if (result2.IsFailure)
            return Result<(T1, T2)>.Failure(result2.Error);

        return Result<(T1, T2)>.Success((result1.Value, result2.Value));
    }

    /// <summary>
    /// Combines three results into a tuple.
    /// </summary>
    public static Result<(T1, T2, T3)> Combine<T1, T2, T3>(
        this Result<T1> result1,
        Result<T2> result2,
        Result<T3> result3)
    {
        if (result1.IsFailure)
            return Result<(T1, T2, T3)>.Failure(result1.Error);

        if (result2.IsFailure)
            return Result<(T1, T2, T3)>.Failure(result2.Error);

        if (result3.IsFailure)
            return Result<(T1, T2, T3)>.Failure(result3.Error);

        return Result<(T1, T2, T3)>.Success((result1.Value, result2.Value, result3.Value));
    }

    /// <summary>
    /// Converts a task result to async.
    /// </summary>
    public static async Task<Result<TNew>> MapAsync<T, TNew>(
        this Task<Result<T>> resultTask,
        Func<T, Task<TNew>> mapper)
    {
        var result = await resultTask;

        if (result.IsFailure)
            return Result<TNew>.Failure(result.Error);

        var newValue = await mapper(result.Value);
        return Result<TNew>.Success(newValue);
    }

    /// <summary>
    /// Binds a task result to async.
    /// </summary>
    public static async Task<Result<TNew>> BindAsync<T, TNew>(
        this Task<Result<T>> resultTask,
        Func<T, Task<Result<TNew>>> binder)
    {
        var result = await resultTask;

        if (result.IsFailure)
            return Result<TNew>.Failure(result.Error);

        return await binder(result.Value);
    }

    /// <summary>
    /// Converts a Maybe to a Result.
    /// </summary>
    public static Result<T> ToResult<T>(this Maybe<T> maybe, Error error)
    {
        return maybe.HasValue
            ? Result<T>.Success(maybe.Value)
            : Result<T>.Failure(error);
    }

    /// <summary>
    /// Converts a Result to a Maybe.
    /// </summary>
    public static Maybe<T> ToMaybe<T>(this Result<T> result)
    {
        return result.IsSuccess
            ? Maybe<T>.Some(result.Value)
            : Maybe<T>.None;
    }

    /// <summary>
    /// Filters a collection of results to only successes.
    /// </summary>
    public static IEnumerable<T> WhereSuccess<T>(this IEnumerable<Result<T>> results)
    {
        return results.Where(r => r.IsSuccess).Select(r => r.Value);
    }

    /// <summary>
    /// Filters a collection of results to only failures.
    /// </summary>
    public static IEnumerable<Error> WhereFailure<T>(this IEnumerable<Result<T>> results)
    {
        return results.Where(r => r.IsFailure).Select(r => r.Error);
    }

    /// <summary>
    /// Aggregates multiple results into a single result containing a list.
    /// </summary>
    public static Result<IReadOnlyList<T>> Aggregate<T>(this IEnumerable<Result<T>> results)
    {
        var list = new List<T>();
        var errors = new List<Error>();

        foreach (var result in results)
        {
            if (result.IsSuccess)
                list.Add(result.Value);
            else
                errors.Add(result.Error);
        }

        if (errors.Count > 0)
        {
            var combinedMessage = string.Join("; ", errors.Select(e => e.Message));
            return Result<IReadOnlyList<T>>.Failure(new Error("Aggregate", combinedMessage));
        }

        return Result<IReadOnlyList<T>>.Success(list.AsReadOnly());
    }
}
