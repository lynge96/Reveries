using System.Text.Json.Serialization;

namespace Reveries.Integration.Isbndb.Dtos;

public sealed record IsbndbBookResponseDto
{
    [JsonPropertyName("book")]
    public IsbndbBookDto? Book { get; init; }
}