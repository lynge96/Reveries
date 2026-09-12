using System.Text.Json.Serialization;

namespace Reveries.Integration.Isbndb.Dtos;

public sealed record IsbndbDimensionDto
{
    [JsonPropertyName("unit")]
    public string? Unit { get; init; }

    [JsonPropertyName("value")]
    public double? Value { get; init; }
}