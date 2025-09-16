using System.Text.Json.Serialization;
using System.Text.Json;

namespace MinimalTaskControl.WebApi.Converters;

public class NullableEnumToStringConverter<T> : JsonConverter<T?> where T : struct, Enum
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var value = reader.GetString();

        if (string.IsNullOrEmpty(value))
            return null;

        if (Enum.TryParse<T>(value, ignoreCase: true, out var result))
        {
            return result;
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.Value.ToString());
    }
}
