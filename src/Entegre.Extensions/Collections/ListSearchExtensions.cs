using System.Linq.Expressions;
using System.Reflection;

namespace Entegre.Extensions;

/// <summary>
/// Provides extension methods for searching and filtering lists.
/// </summary>
public static class ListSearchExtensions
{
    /// <summary>
    /// Searches for items containing the query in any of the specified properties.
    /// </summary>
    public static IEnumerable<T> Search<T>(
        this IEnumerable<T> source,
        string query,
        params Expression<Func<T, string?>>[] propertySelectors)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (string.IsNullOrWhiteSpace(query))
            return source;

        var compiledSelectors = propertySelectors
            .Select(s => s.Compile())
            .ToList();

        return source.Where(item =>
            compiledSelectors.Any(selector =>
                selector(item)?.Contains(query, StringComparison.OrdinalIgnoreCase) == true));
    }

    /// <summary>
    /// Performs fuzzy search on items using the specified properties.
    /// </summary>
    public static IEnumerable<T> FuzzySearch<T>(
        this IEnumerable<T> source,
        string query,
        double threshold = 0.6,
        params Expression<Func<T, string?>>[] propertySelectors)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (string.IsNullOrWhiteSpace(query))
            return source;

        var compiledSelectors = propertySelectors
            .Select(s => s.Compile())
            .ToList();

        return source
            .Select(item => new
            {
                Item = item,
                Score = compiledSelectors
                    .Select(selector => CalculateSimilarity(query, selector(item) ?? ""))
                    .Max()
            })
            .Where(x => x.Score >= threshold)
            .OrderByDescending(x => x.Score)
            .Select(x => x.Item);
    }

    /// <summary>
    /// Applies a conditional where clause.
    /// </summary>
    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        return condition ? source.Where(predicate) : source;
    }

    /// <summary>
    /// Applies a conditional where clause with lazy condition evaluation.
    /// </summary>
    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source,
        Func<bool> condition,
        Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        return condition() ? source.Where(predicate) : source;
    }

    /// <summary>
    /// Determines whether the enumerable contains any of the specified items.
    /// </summary>
    public static bool ContainsAny<T>(this IEnumerable<T> source, params T[] items)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(items);

        var itemSet = new HashSet<T>(items);
        return source.Any(itemSet.Contains);
    }

    /// <summary>
    /// Determines whether the enumerable contains all of the specified items.
    /// </summary>
    public static bool ContainsAll<T>(this IEnumerable<T> source, params T[] items)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(items);

        var sourceSet = new HashSet<T>(source);
        return items.All(sourceSet.Contains);
    }

    /// <summary>
    /// Finds duplicate items in the enumerable.
    /// </summary>
    public static IEnumerable<T> FindDuplicates<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return source
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);
    }

    /// <summary>
    /// Finds duplicate items by a key selector.
    /// </summary>
    public static IEnumerable<T> FindDuplicatesBy<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        return source
            .GroupBy(keySelector)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g);
    }

    /// <summary>
    /// Orders the enumerable dynamically by property name.
    /// </summary>
    public static IOrderedEnumerable<T> OrderByDynamic<T>(
        this IEnumerable<T> source,
        string propertyName,
        bool descending = false)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        var property = typeof(T).GetProperty(propertyName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (property is null)
            throw new ArgumentException($"Property '{propertyName}' not found on type '{typeof(T).Name}'");

        return descending
            ? source.OrderByDescending(x => property.GetValue(x))
            : source.OrderBy(x => property.GetValue(x));
    }

    /// <summary>
    /// Calculates the similarity between two strings using Levenshtein distance.
    /// </summary>
    private static double CalculateSimilarity(string source, string target)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            return 0;

        source = source.ToLowerInvariant();
        target = target.ToLowerInvariant();

        if (source == target)
            return 1.0;

        var sourceLength = source.Length;
        var targetLength = target.Length;

        if (sourceLength == 0 || targetLength == 0)
            return 0;

        var distance = new int[sourceLength + 1, targetLength + 1];

        for (var i = 0; i <= sourceLength; i++)
            distance[i, 0] = i;

        for (var j = 0; j <= targetLength; j++)
            distance[0, j] = j;

        for (var i = 1; i <= sourceLength; i++)
        {
            for (var j = 1; j <= targetLength; j++)
            {
                var cost = source[i - 1] == target[j - 1] ? 0 : 1;
                distance[i, j] = Math.Min(
                    Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                    distance[i - 1, j - 1] + cost);
            }
        }

        var maxLength = Math.Max(sourceLength, targetLength);
        return 1.0 - (double)distance[sourceLength, targetLength] / maxLength;
    }
}
