using System.Text.Json.Serialization;
using Reveries.Integration.Isbndb.DTOs.Books;

namespace Reveries.Integration.Isbndb;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(IsbndbBookResponseDto))]
[JsonSerializable(typeof(IsbndbBookSearchResponseDto))]
[JsonSerializable(typeof(IsbndbBookListResponseDto))]
[JsonSerializable(typeof(IsbndbBulkIsbnRequest))]
internal sealed partial class IsbndbJsonContext : JsonSerializerContext;