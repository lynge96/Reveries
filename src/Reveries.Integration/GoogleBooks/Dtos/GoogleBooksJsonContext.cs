using System.Text.Json.Serialization;

namespace Reveries.Integration.GoogleBooks.Dtos;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(GoogleBookResponseDto))]
[JsonSerializable(typeof(GoogleBookItemDto))]
internal sealed partial class GoogleBooksJsonContext : JsonSerializerContext;