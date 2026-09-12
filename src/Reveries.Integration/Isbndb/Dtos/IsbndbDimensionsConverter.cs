using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reveries.Integration.Isbndb.Dtos;

public sealed class IsbndbDimensionsConverter : JsonConverter<IsbndbDimensionsDto?>
{
    public override IsbndbDimensionsDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            reader.Skip();
            return null;
        }

        return JsonSerializer.Deserialize(ref reader, IsbndbJsonContext.Default.IsbndbDimensionsDto);
    }

    public override void Write(Utf8JsonWriter writer, IsbndbDimensionsDto? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            JsonSerializer.Serialize(writer, value, IsbndbJsonContext.Default.IsbndbDimensionsDto);
    }
}