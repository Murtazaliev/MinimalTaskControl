using System.Text.Json.Serialization;
using System.Text.Json;

namespace MinimalTaskControl.WebApi.Converters;

public class EnumToStringConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (Enum.TryParse<T>(value, ignoreCase: true, out var result))
        {
            return result;
        }

        return default;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
