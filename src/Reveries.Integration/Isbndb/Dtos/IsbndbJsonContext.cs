using System.Text.Json.Serialization;

namespace Reveries.Integration.Isbndb.Dtos;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(IsbndbBookResponseDto))]
[JsonSerializable(typeof(IsbndbBookListResponseDto))]
[JsonSerializable(typeof(IsbndbBulkIsbnRequest))]
[JsonSerializable(typeof(IsbndbDimensionsDto))]
internal sealed partial class IsbndbJsonContext : JsonSerializerContext;