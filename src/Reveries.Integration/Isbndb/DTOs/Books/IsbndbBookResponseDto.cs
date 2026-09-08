using System.Text.Json.Serialization;

namespace Reveries.Integration.Isbndb.DTOs.Books;

public sealed record IsbndbBookResponseDto
{
    [JsonPropertyName("book")]
    public IsbndbBookDto? Book { get; init; }
}