namespace Entegre.Extensions;

/// <summary>
/// Provides extension methods for collections.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Determines whether the collection is null or empty.
    /// </summary>
    public static bool IsNullOrEmpty<T>(this ICollection<T>? collection)
    {
        return collection is null || collection.Count == 0;
    }

    /// <summary>
    /// Determines whether the enumerable is null or empty.
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? enumerable)
    {
        return enumerable is null || !enumerable.Any();
    }

    /// <summary>
    /// Adds multiple items to the collection.
    /// </summary>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(items);

        if (collection is List<T> list)
        {
            list.AddRange(items);
        }
        else
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }

    /// <summary>
    /// Adds an item to the collection if the condition is true.
    /// </summary>
    public static bool AddIf<T>(this ICollection<T> collection, T item, bool condition)
    {
        ArgumentNullException.ThrowIfNull(collection);

        if (condition)
        {
            collection.Add(item);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds an item to the collection if the predicate returns true.
    /// </summary>
    public static bool AddIf<T>(this ICollection<T> collection, T item, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(predicate);

        if (predicate(item))
        {
            collection.Add(item);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes all items matching the predicate from the collection.
    /// </summary>
    public static int RemoveAll<T>(this ICollection<T> collection, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(predicate);

        if (collection is List<T> list)
        {
            return list.RemoveAll(new Predicate<T>(predicate));
        }

        var itemsToRemove = collection.Where(predicate).ToList();
        foreach (var item in itemsToRemove)
        {
            collection.Remove(item);
        }

        return itemsToRemove.Count;
    }

    /// <summary>
    /// Replaces all occurrences of an item with a new item.
    /// </summary>
    public static int ReplaceAll<T>(this IList<T> list, T oldItem, T newItem)
    {
        ArgumentNullException.ThrowIfNull(list);

        var count = 0;
        for (var i = 0; i < list.Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(list[i], oldItem))
            {
                list[i] = newItem;
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// Executes an action on each item in the collection.
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(enumerable);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in enumerable)
        {
            action(item);
        }
    }

    /// <summary>
    /// Executes an action on each item in the collection with index.
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
    {
        ArgumentNullException.ThrowIfNull(enumerable);
        ArgumentNullException.ThrowIfNull(action);

        var index = 0;
        foreach (var item in enumerable)
        {
            action(item, index++);
        }
    }

    /// <summary>
    /// Converts the enumerable to a HashSet.
    /// </summary>
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable, IEqualityComparer<T>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(enumerable);
        return new HashSet<T>(enumerable, comparer);
    }
}
