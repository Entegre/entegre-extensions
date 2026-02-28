using System.Dynamic;
using System.Reflection;

namespace Entegre.Extensions;

/// <summary>
/// Provides extension methods for dynamic querying.
/// </summary>
public static class DynamicQueryExtensions
{
    /// <summary>
    /// Converts the enumerable to a list of dynamic objects.
    /// </summary>
    public static List<dynamic> ToDynamicList<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return source.Select(item =>
        {
            var expando = new ExpandoObject() as IDictionary<string, object?>;
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                expando[prop.Name] = prop.GetValue(item);
            }

            return (dynamic)expando;
        }).ToList();
    }

    /// <summary>
    /// Selects specific properties dynamically.
    /// </summary>
    public static IEnumerable<dynamic> SelectDynamic<T>(
        this IEnumerable<T> source,
        params string[] propertyNames)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(propertyNames);

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => propertyNames.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
            .ToList();

        return source.Select(item =>
        {
            var expando = new ExpandoObject() as IDictionary<string, object?>;

            foreach (var prop in properties)
            {
                expando[prop.Name] = prop.GetValue(item);
            }

            return (dynamic)expando;
        });
    }

    /// <summary>
    /// Groups by a property dynamically.
    /// </summary>
    public static IEnumerable<IGrouping<object?, T>> GroupByDynamic<T>(
        this IEnumerable<T> source,
        string propertyName)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        var property = typeof(T).GetProperty(propertyName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (property is null)
            throw new ArgumentException($"Property '{propertyName}' not found on type '{typeof(T).Name}'");

        return source.GroupBy(x => property.GetValue(x));
    }

    /// <summary>
    /// Creates a pivot table from the enumerable.
    /// </summary>
    public static IEnumerable<dynamic> ToPivot<T, TRow, TColumn, TValue>(
        this IEnumerable<T> source,
        Func<T, TRow> rowSelector,
        Func<T, TColumn> columnSelector,
        Func<IEnumerable<T>, TValue> valueAggregator)
        where TColumn : notnull
    {
        ArgumentNullException.ThrowIfNull(source);

        var groups = source.GroupBy(rowSelector);

        foreach (var rowGroup in groups)
        {
            var expando = new ExpandoObject() as IDictionary<string, object?>;
            expando["RowKey"] = rowGroup.Key;

            var columnGroups = rowGroup.GroupBy(columnSelector);
            foreach (var colGroup in columnGroups)
            {
                var columnName = colGroup.Key?.ToString() ?? "null";
                expando[columnName] = valueAggregator(colGroup);
            }

            yield return (dynamic)expando;
        }
    }

    /// <summary>
    /// Converts the enumerable to a lookup dictionary for fast access.
    /// </summary>
    public static Dictionary<TKey, T> ToLookupDictionary<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey> keySelector)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        return source.ToDictionary(keySelector);
    }

    /// <summary>
    /// Converts the enumerable to a lookup dictionary with a value selector.
    /// </summary>
    public static Dictionary<TKey, TValue> ToLookupDictionary<T, TKey, TValue>(
        this IEnumerable<T> source,
        Func<T, TKey> keySelector,
        Func<T, TValue> valueSelector)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(valueSelector);

        return source.ToDictionary(keySelector, valueSelector);
    }

    /// <summary>
    /// Converts the enumerable to a multi-value lookup dictionary.
    /// </summary>
    public static Dictionary<TKey, List<T>> ToMultiValueDictionary<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey> keySelector)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        return source.GroupBy(keySelector)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}
