using System.Text.Json.Serialization;
using Reveries.Integration.GoogleBooks.DTOs;

namespace Reveries.Integration.GoogleBooks;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(GoogleBookResponseDto))]
[JsonSerializable(typeof(GoogleBookItemDto))]
internal sealed partial class GoogleBooksJsonContext : JsonSerializerContext;