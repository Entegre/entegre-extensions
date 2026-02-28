using System.Text.Json;
using System.Text.Json.Nodes;

namespace Entegre.Extensions;

/// <summary>
/// Provides extension methods for JSON operations.
/// </summary>
public static class JsonExtensions
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    private static readonly JsonSerializerOptions PrettyOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    /// <summary>
    /// Serializes the object to JSON.
    /// </summary>
    public static string ToJson<T>(this T obj, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize(obj, options ?? DefaultOptions);
    }

    /// <summary>
    /// Deserializes the JSON string to an object.
    /// </summary>
    public static T? FromJson<T>(this string json, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(json);
        return JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);
    }

    /// <summary>
    /// Serializes the object to pretty-printed JSON.
    /// </summary>
    public static string ToJsonPretty<T>(this T obj, JsonSerializerOptions? options = null)
    {
        var opts = options ?? PrettyOptions;
        if (options is not null)
        {
            opts = new JsonSerializerOptions(options) { WriteIndented = true };
        }

        return JsonSerializer.Serialize(obj, opts);
    }

    /// <summary>
    /// Tries to deserialize the JSON string to an object.
    /// </summary>
    public static bool TryFromJson<T>(this string? json, out T? result, JsonSerializerOptions? options = null)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            result = JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Merges two JSON objects.
    /// </summary>
    public static string MergeJson(this string json1, string json2)
    {
        ArgumentNullException.ThrowIfNull(json1);
        ArgumentNullException.ThrowIfNull(json2);

        var node1 = JsonNode.Parse(json1);
        var node2 = JsonNode.Parse(json2);

        if (node1 is JsonObject obj1 && node2 is JsonObject obj2)
        {
            MergeJsonObjects(obj1, obj2);
            return obj1.ToJsonString();
        }

        throw new ArgumentException("Both JSON strings must represent objects");
    }

    /// <summary>
    /// Gets a value from JSON by path (e.g., "person.address.city").
    /// </summary>
    public static T? GetJsonValue<T>(this string json, string path)
    {
        ArgumentNullException.ThrowIfNull(json);
        ArgumentNullException.ThrowIfNull(path);

        var node = JsonNode.Parse(json);
        var segments = path.Split('.');

        foreach (var segment in segments)
        {
            if (node is null)
                return default;

            // Check for array index
            if (segment.EndsWith(']') && segment.Contains('['))
            {
                var bracketIndex = segment.IndexOf('[');
                var propertyName = segment[..bracketIndex];
                var indexStr = segment[(bracketIndex + 1)..^1];

                if (!string.IsNullOrEmpty(propertyName))
                {
                    node = node[propertyName];
                }

                if (int.TryParse(indexStr, out var index) && node is JsonArray array)
                {
                    node = array[index];
                }
            }
            else
            {
                node = node[segment];
            }
        }

        if (node is null)
            return default;

        return node.Deserialize<T>(DefaultOptions);
    }

    /// <summary>
    /// Gets a string value from JSON by path.
    /// </summary>
    public static string? GetJsonValue(this string json, string path)
    {
        return json.GetJsonValue<string>(path);
    }

    /// <summary>
    /// Validates whether the string is valid JSON.
    /// </summary>
    public static bool IsValidJson(this string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;

        json = json.Trim();
        if ((!json.StartsWith('{') || !json.EndsWith('}')) &&
            (!json.StartsWith('[') || !json.EndsWith(']')))
            return false;

        try
        {
            JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void MergeJsonObjects(JsonObject target, JsonObject source)
    {
        foreach (var property in source)
        {
            if (target.ContainsKey(property.Key))
            {
                if (target[property.Key] is JsonObject targetObj &&
                    property.Value is JsonObject sourceObj)
                {
                    MergeJsonObjects(targetObj, sourceObj);
                }
                else
                {
                    target[property.Key] = property.Value?.DeepClone();
                }
            }
            else
            {
                target[property.Key] = property.Value?.DeepClone();
            }
        }
    }
}
