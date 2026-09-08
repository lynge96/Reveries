using System.Text.Json.Serialization;

namespace Reveries.Integration.Isbndb.DTOs.Books;

public sealed record IsbndbDimensionDto
{
    [JsonPropertyName("unit")]
    public string? Unit { get; init; }

    [JsonPropertyName("value")]
    public double? Value { get; init; }
}