using System.Security.Cryptography;

namespace Entegre.Extensions;

/// <summary>
/// Provides extension methods for IEnumerable.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Splits the enumerable into batches of the specified size.
    /// </summary>
    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(size, 0);

        return source.Select((item, index) => (item, index))
            .GroupBy(x => x.index / size)
            .Select(g => g.Select(x => x.item));
    }

    /// <summary>
    /// Filters out null values from the enumerable.
    /// </summary>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.Where(item => item is not null)!;
    }

    /// <summary>
    /// Filters out null values from the enumerable of nullable value types.
    /// </summary>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : struct
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.Where(item => item.HasValue).Select(item => item!.Value);
    }

    /// <summary>
    /// Returns distinct elements by a specified key selector.
    /// </summary>
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        var seen = new HashSet<TKey>();
        foreach (var item in source)
        {
            if (seen.Add(keySelector(item)))
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Returns the first element or the default value if the sequence is empty.
    /// </summary>
    public static T? SafeFirst<T>(this IEnumerable<T>? source)
    {
        if (source is null)
            return default;

        return source.FirstOrDefault();
    }

    /// <summary>
    /// Returns the first element matching the predicate or the default value.
    /// </summary>
    public static T? SafeFirst<T>(this IEnumerable<T>? source, Func<T, bool> predicate)
    {
        if (source is null)
            return default;

        return source.FirstOrDefault(predicate);
    }

    /// <summary>
    /// Returns the last element or the default value if the sequence is empty.
    /// </summary>
    public static T? SafeLast<T>(this IEnumerable<T>? source)
    {
        if (source is null)
            return default;

        return source.LastOrDefault();
    }

    /// <summary>
    /// Returns an empty enumerable if the source is null.
    /// </summary>
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source)
    {
        return source ?? Enumerable.Empty<T>();
    }

    /// <summary>
    /// Shuffles the enumerable using a cryptographically secure random number generator.
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var list = source.ToList();
        var n = list.Count;

        while (n > 1)
        {
            n--;
            var k = RandomNumberGenerator.GetInt32(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }

        return list;
    }

    /// <summary>
    /// Takes a random sample of items from the enumerable.
    /// </summary>
    public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> source, int count)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        return source.Shuffle().Take(count);
    }

    /// <summary>
    /// Flattens a nested enumerable of enumerables.
    /// </summary>
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.SelectMany(x => x);
    }

    /// <summary>
    /// Adds an index to each element in the enumerable.
    /// </summary>
    public static IEnumerable<(T Item, int Index)> Index<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.Select((item, index) => (item, index));
    }

    /// <summary>
    /// Partitions the enumerable into two groups based on a predicate.
    /// </summary>
    public static (IEnumerable<T> Matching, IEnumerable<T> NonMatching) Partition<T>(
        this IEnumerable<T> source,
        Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        var groups = source.GroupBy(predicate).ToDictionary(g => g.Key, g => g.AsEnumerable());

        return (
            groups.GetValueOrDefault(true, Enumerable.Empty<T>()),
            groups.GetValueOrDefault(false, Enumerable.Empty<T>())
        );
    }

    /// <summary>
    /// Returns the element with the minimum value of the specified selector.
    /// </summary>
    public static T? MinByOrDefault<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        where TKey : IComparable<TKey>
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        using var enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
            return default;

        var minItem = enumerator.Current;
        var minKey = keySelector(minItem);

        while (enumerator.MoveNext())
        {
            var currentKey = keySelector(enumerator.Current);
            if (currentKey.CompareTo(minKey) < 0)
            {
                minItem = enumerator.Current;
                minKey = currentKey;
            }
        }

        return minItem;
    }

    /// <summary>
    /// Returns the element with the maximum value of the specified selector.
    /// </summary>
    public static T? MaxByOrDefault<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        where TKey : IComparable<TKey>
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        using var enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
            return default;

        var maxItem = enumerator.Current;
        var maxKey = keySelector(maxItem);

        while (enumerator.MoveNext())
        {
            var currentKey = keySelector(enumerator.Current);
            if (currentKey.CompareTo(maxKey) > 0)
            {
                maxItem = enumerator.Current;
                maxKey = currentKey;
            }
        }

        return maxItem;
    }

    /// <summary>
    /// Converts the enumerable to a read-only collection.
    /// </summary>
    public static IReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.ToList().AsReadOnly();
    }
}
