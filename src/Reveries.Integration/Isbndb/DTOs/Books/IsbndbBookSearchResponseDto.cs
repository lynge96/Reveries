using System.Text.Json.Serialization;

namespace Reveries.Integration.Isbndb.DTOs.Books;

public sealed record IsbndbBookSearchResponseDto
{
    [JsonPropertyName("total")]
    public int Total { get; init; }

    [JsonPropertyName("books")]
    public IReadOnlyList<IsbndbBookDto>? Books { get; init; }
}