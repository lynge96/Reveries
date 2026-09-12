using System.Text.Json.Serialization;

namespace Reveries.Integration.Isbndb.Dtos;

public sealed record IsbndbBookDto
{
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("isbn13")]
    public string? Isbn13 { get; init; }

    [JsonPropertyName("isbn10")]
    public string? Isbn10 { get; init; }

    [JsonPropertyName("authors")]
    public IReadOnlyList<string>? Authors { get; init; }

    [JsonPropertyName("publisher")]
    public string? Publisher { get; init; }

    [JsonPropertyName("synopsis")]
    public string? Synopsis { get; init; }

    [JsonPropertyName("subjects")]
    public IReadOnlyList<string>? Subjects { get; init; }

    [JsonPropertyName("dewey_decimal")]
    public IReadOnlyList<string>? DeweyDecimals { get; init; }

    [JsonPropertyName("edition")]
    public string? Edition { get; init; }

    [JsonPropertyName("binding")]
    public string? Binding { get; init; }

    [JsonPropertyName("language")]
    public string? Language { get; init; }

    [JsonPropertyName("pages")]
    public int? Pages { get; init; }

    [JsonPropertyName("date_published")]
    public string? DatePublished { get; init; }

    [JsonPropertyName("image")]
    public string? Image { get; init; }

    [JsonPropertyName("image_original")]
    public string? ImageOriginal { get; init; }

    [JsonPropertyName("dimensions_structured")]
    [JsonConverter(typeof(IsbndbDimensionsConverter))]
    public IsbndbDimensionsDto? DimensionsStructured { get; init; }
}