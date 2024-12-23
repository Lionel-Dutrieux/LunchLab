using System.Text.Json;
using System.Text.Json.Serialization;

namespace PayloadClient.Converters;

/// <summary>
/// Converts between JSON and entity reference objects, handling both string IDs and full object representations.
/// </summary>
/// <typeparam name="T">The type of entity reference to convert. Must be a class with a parameterless constructor and an Id property.</typeparam>
/// <remarks>
/// This converter handles two JSON formats:
/// 1. A string containing just the entity ID: "123456"
/// 2. A full object with all properties: { "id": "123456", "name": "Example", ... }
/// 
/// Example usage:
/// [JsonConverter(typeof(EntityRefConverter&lt;UserRef&gt;))]
/// public UserRef User { get; set; }
/// </remarks>
public class EntityRefConverter<T> : JsonConverter<T> where T : class, new()
{
    /// <summary>
    /// Reads JSON data and converts it to an entity reference object.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type to convert to.</param>
    /// <param name="options">The serialization options.</param>
    /// <returns>An instance of T containing either just the ID or all properties.</returns>
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            // If it's just an ID string, create a new instance with just the ID
            var id = reader.GetString();
            var instance = new T();
            typeof(T).GetProperty("Id")?.SetValue(instance, id);
            return instance;
        }
        
        // Otherwise deserialize the full object
        return JsonSerializer.Deserialize<T>(ref reader, options);
    }

    /// <summary>
    /// Writes an entity reference object to JSON.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="options">The serialization options.</param>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
} 