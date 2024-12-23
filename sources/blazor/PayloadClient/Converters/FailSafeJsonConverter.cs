using System.Text.Json;
using System.Text.Json.Serialization;

namespace PayloadClient.Converters;

public class FailSafeJsonConverter<T> : JsonConverter<T> where T : class, new()
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return new T();
            }
            
            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            return jsonDoc.RootElement.Deserialize<T>(options);
        }
        catch
        {
            return new T();
        }
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
} 