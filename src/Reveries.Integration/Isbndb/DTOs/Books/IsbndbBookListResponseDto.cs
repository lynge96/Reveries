using System.Text.Json.Serialization;

namespace Reveries.Integration.Isbndb.DTOs.Books;

public sealed record IsbndbBookListResponseDto
{
    [JsonPropertyName("total")]
    public int Total { get; init; }

    [JsonPropertyName("requested")]
    public int Requested { get; init; }

    [JsonPropertyName("data")]
    public IReadOnlyList<IsbndbBookDto>? Data { get; init; }
}