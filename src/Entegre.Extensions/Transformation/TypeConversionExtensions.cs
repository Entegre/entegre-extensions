using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Entegre.Extensions;

/// <summary>
/// Provides extension methods for type conversion.
/// </summary>
public static class TypeConversionExtensions
{
    private static readonly HashSet<string> TrueValues = new(StringComparer.OrdinalIgnoreCase)
    {
        "true", "1", "yes", "on", "enabled", "active"
    };

    private static readonly HashSet<string> FalseValues = new(StringComparer.OrdinalIgnoreCase)
    {
        "false", "0", "no", "off", "disabled", "inactive"
    };

    /// <summary>
    /// Converts the value to the specified type.
    /// </summary>
    public static T? To<T>(this object? value)
    {
        return (T?)To(value, typeof(T));
    }

    /// <summary>
    /// Converts the value to the specified type, or returns the default value.
    /// </summary>
    public static T ToOrDefault<T>(this object? value, T defaultValue = default!)
    {
        try
        {
            var result = To<T>(value);
            return result ?? defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Tries to convert the value to the specified type.
    /// </summary>
    public static bool TryConvert<T>(this object? value, out T? result)
    {
        try
        {
            result = To<T>(value);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Converts the value to an integer.
    /// </summary>
    public static int ToInt(this object? value, int defaultValue = 0)
    {
        return value.ToOrDefault(defaultValue);
    }

    /// <summary>
    /// Converts the value to a long.
    /// </summary>
    public static long ToLong(this object? value, long defaultValue = 0)
    {
        return value.ToOrDefault(defaultValue);
    }

    /// <summary>
    /// Converts the value to a decimal.
    /// </summary>
    public static decimal ToDecimal(this object? value, decimal defaultValue = 0)
    {
        return value.ToOrDefault(defaultValue);
    }

    /// <summary>
    /// Converts the value to a double.
    /// </summary>
    public static double ToDouble(this object? value, double defaultValue = 0)
    {
        return value.ToOrDefault(defaultValue);
    }

    /// <summary>
    /// Converts the value to a boolean.
    /// </summary>
    public static bool ToBool(this object? value, bool defaultValue = false)
    {
        if (value is null)
            return defaultValue;

        if (value is bool b)
            return b;

        var strValue = value.ToString();
        if (string.IsNullOrWhiteSpace(strValue))
            return defaultValue;

        if (TrueValues.Contains(strValue))
            return true;

        if (FalseValues.Contains(strValue))
            return false;

        return defaultValue;
    }

    /// <summary>
    /// Converts the value to a GUID.
    /// </summary>
    public static Guid ToGuid(this object? value)
    {
        if (value is null)
            return Guid.Empty;

        if (value is Guid g)
            return g;

        var strValue = value.ToString();
        if (Guid.TryParse(strValue, out var result))
            return result;

        return Guid.Empty;
    }

    /// <summary>
    /// Converts the value to an enum.
    /// </summary>
    public static TEnum ToEnum<TEnum>(this object? value, TEnum defaultValue = default) where TEnum : struct, Enum
    {
        if (value is null)
            return defaultValue;

        if (value is TEnum e)
            return e;

        var strValue = value.ToString();
        if (string.IsNullOrWhiteSpace(strValue))
            return defaultValue;

        if (Enum.TryParse<TEnum>(strValue, true, out var result))
            return result;

        // Try to parse by description attribute
        var fields = typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in fields)
        {
            var description = field.GetCustomAttribute<DescriptionAttribute>();
            if (description?.Description.Equals(strValue, StringComparison.OrdinalIgnoreCase) == true)
            {
                return (TEnum)field.GetValue(null)!;
            }
        }

        // Try numeric conversion
        if (int.TryParse(strValue, out var intValue) && Enum.IsDefined(typeof(TEnum), intValue))
        {
            return (TEnum)(object)intValue;
        }

        return defaultValue;
    }

    /// <summary>
    /// Converts the value to a DateTime.
    /// </summary>
    public static DateTime ToDateTime(this object? value, DateTime defaultValue = default)
    {
        if (value is null)
            return defaultValue;

        if (value is DateTime dt)
            return dt;

        if (value is DateTimeOffset dto)
            return dto.DateTime;

        var strValue = value.ToString();
        if (DateTime.TryParse(strValue, out var result))
            return result;

        return defaultValue;
    }

    private static object? To(object? value, Type targetType)
    {
        if (value is null)
        {
            return targetType.IsValueType && Nullable.GetUnderlyingType(targetType) is null
                ? Activator.CreateInstance(targetType)
                : null;
        }

        var valueType = value.GetType();

        // Handle same type
        if (targetType.IsAssignableFrom(valueType))
            return value;

        // Handle nullable types
        var underlyingType = Nullable.GetUnderlyingType(targetType);
        if (underlyingType is not null)
        {
            targetType = underlyingType;
        }

        // Handle string
        if (targetType == typeof(string))
            return value.ToString();

        // Handle Guid
        if (targetType == typeof(Guid))
        {
            if (value is string strValue && Guid.TryParse(strValue, out var guid))
                return guid;
            throw new InvalidCastException($"Cannot convert '{value}' to Guid");
        }

        // Handle enums
        if (targetType.IsEnum)
        {
            if (value is string strValue)
                return Enum.Parse(targetType, strValue, true);
            return Enum.ToObject(targetType, value);
        }

        // Handle DateTime
        if (targetType == typeof(DateTime))
        {
            if (value is string strValue && DateTime.TryParse(strValue, out var dt))
                return dt;
            if (value is long timestamp)
                return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
        }

        // Handle basic type conversions
        if (value is IConvertible)
        {
            return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }

        throw new InvalidCastException($"Cannot convert from '{valueType.Name}' to '{targetType.Name}'");
    }
}
