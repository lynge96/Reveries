using System.Text.Json.Serialization;
using Reveries.Core.Enums;

namespace Reveries.Application.DTOs.Books;

public class BookDto
{
    public required string Title { get; init; } = string.Empty; // giver mening at kræve Title
    public string? Image { get; init; } // kan godt være null fra API’et
    [JsonPropertyName("image_original")]
    public string? ImageOriginal { get; init; }
    [JsonPropertyName("title_long")]
    public string? TitleLong { get; init; }
    [JsonPropertyName("date_published")]
    public string? DatePublished { get; init; }

    public string? Publisher { get; init; }
    public string? Synopsis { get; init; }
    public IEnumerable<string>? Subjects { get; init; }
    public IEnumerable<string>? Authors { get; init; }
    
    [JsonPropertyName("dewey_decimal")]
    public ICollection<string>? DeweyDecimals { get; init; }

    public required string Isbn13 { get; init; }
    public string? Isbn { get; init; }
    public string? Isbn10 { get; init; }
    public string? Edition { get; init; }
    public string? Binding { get; init; }
    public decimal? Msrp { get; init; }
    public string? Language { get; init; }
    public string? Dimensions { get; init; }
    public int? Pages { get; init; }

    [JsonPropertyName("dimensions_structured")]
    public DimensionsStructuredDto? DimensionsStructured { get; init; }
}

