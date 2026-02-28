using System.Collections;
using System.Reflection;
using System.Text.Json;

namespace Entegre.Extensions;

/// <summary>
/// Provides extension methods for object mapping and cloning.
/// </summary>
public static class ObjectMapperExtensions
{
    /// <summary>
    /// Maps the object to a new instance of the target type.
    /// </summary>
    public static TTarget MapTo<TTarget>(this object source) where TTarget : new()
    {
        ArgumentNullException.ThrowIfNull(source);

        var target = new TTarget();
        MapProperties(source, target);
        return target;
    }

    /// <summary>
    /// Maps the object to an existing target instance.
    /// </summary>
    public static TTarget MapTo<TTarget>(this object source, TTarget target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        MapProperties(source, target);
        return target;
    }

    /// <summary>
    /// Maps a collection of objects to a list of target type.
    /// </summary>
    public static List<TTarget> MapToList<TTarget>(this IEnumerable source) where TTarget : new()
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = new List<TTarget>();
        foreach (var item in source)
        {
            if (item is not null)
            {
                result.Add(item.MapTo<TTarget>());
            }
        }

        return result;
    }

    /// <summary>
    /// Creates a deep clone of the object using JSON serialization.
    /// </summary>
    public static T? CloneDeep<T>(this T source)
    {
        if (source is null)
            return default;

        var json = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    /// Creates a shallow clone of the object using MemberwiseClone.
    /// </summary>
    public static T? CloneShallow<T>(this T source) where T : class
    {
        if (source is null)
            return default;

        var method = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
        return method?.Invoke(source, null) as T;
    }

    /// <summary>
    /// Converts the object to a dictionary of property names and values.
    /// </summary>
    public static Dictionary<string, object?> ToDictionary(this object source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var properties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        foreach (var prop in properties)
        {
            if (prop.CanRead)
            {
                result[prop.Name] = prop.GetValue(source);
            }
        }

        return result;
    }

    /// <summary>
    /// Creates an object from a dictionary of property names and values.
    /// </summary>
    public static T FromDictionary<T>(this IDictionary<string, object?> dictionary) where T : new()
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        var target = new T();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (prop.CanWrite && dictionary.TryGetValue(prop.Name, out var value))
            {
                try
                {
                    var convertedValue = ConvertValue(value, prop.PropertyType);
                    prop.SetValue(target, convertedValue);
                }
                catch
                {
                    // Skip properties that can't be converted
                }
            }
        }

        return target;
    }

    /// <summary>
    /// Gets the differences between two objects.
    /// </summary>
    public static Dictionary<string, (object? OldValue, object? NewValue)> Diff<T>(this T source, T other)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(other);

        var result = new Dictionary<string, (object? OldValue, object? NewValue)>();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (!prop.CanRead)
                continue;

            var sourceValue = prop.GetValue(source);
            var otherValue = prop.GetValue(other);

            if (!Equals(sourceValue, otherValue))
            {
                result[prop.Name] = (sourceValue, otherValue);
            }
        }

        return result;
    }

    private static void MapProperties(object source, object target)
    {
        var sourceType = source.GetType();
        var targetType = target.GetType();

        var targetProperties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        var sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead);

        foreach (var sourceProp in sourceProperties)
        {
            if (targetProperties.TryGetValue(sourceProp.Name, out var targetProp))
            {
                try
                {
                    var value = sourceProp.GetValue(source);
                    var convertedValue = ConvertValue(value, targetProp.PropertyType);
                    targetProp.SetValue(target, convertedValue);
                }
                catch
                {
                    // Skip properties that can't be mapped
                }
            }
        }
    }

    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value is null)
            return null;

        var valueType = value.GetType();

        if (targetType.IsAssignableFrom(valueType))
            return value;

        // Handle nullable types
        var underlyingType = Nullable.GetUnderlyingType(targetType);
        if (underlyingType is not null)
        {
            targetType = underlyingType;
        }

        // Handle enums
        if (targetType.IsEnum)
        {
            if (value is string strValue)
                return Enum.Parse(targetType, strValue, true);
            return Enum.ToObject(targetType, value);
        }

        // Handle basic conversions
        if (targetType == typeof(string))
            return value.ToString();

        if (value is IConvertible)
            return Convert.ChangeType(value, targetType);

        return value;
    }
}
