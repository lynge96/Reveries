using System.Text.Json.Serialization;

namespace Reveries.Integration.Isbndb.DTOs.Books;

public sealed record IsbndbDimensionsDto
{
    [JsonPropertyName("length")]
    public IsbndbDimensionDto? Length { get; init; }

    [JsonPropertyName("width")]
    public IsbndbDimensionDto? Width { get; init; }

    [JsonPropertyName("height")]
    public IsbndbDimensionDto? Height { get; init; }

    [JsonPropertyName("weight")]
    public IsbndbDimensionDto? Weight { get; init; }
}