using System.Text.Json.Serialization;
using Reveries.Core.Models;

namespace Reveries.Core.DTOs.Books;

public class BookDto
{
    public required string Title { get; init; } = string.Empty;
    
    public string Image { get; init; } = string.Empty;
    [JsonPropertyName("image_original")]
    public string ImageOriginal { get; init; } = string.Empty;
    [JsonPropertyName("title_long")]
    public string TitleLong { get; init; } = string.Empty;
    [JsonPropertyName("date_published")]
    public string DatePublished { get; init; } = string.Empty;
    
    public string Publisher { get; init; } = string.Empty;
    
    public string Synopsis { get; init; } = string.Empty;
    
    public IEnumerable<string> Subjects { get; init; } = null!;
    
    public IEnumerable<string> Authors { get; init; } = null!;
    
    public required string Isbn13 { get; init; } = string.Empty;
    
    public required string Isbn { get; init; } = string.Empty;
    
    public string Isbn10 { get; init; } = string.Empty;
    
    // public string Edition { get; init; } = string.Empty;
    
    public string Binding { get; init; } = string.Empty;
    
    public decimal Msrp { get; init; }
    
    public string Language { get; init; } = string.Empty;
    
    public string Dimensions { get; init; } = string.Empty;
    
    public int Pages { get; init; }
    [JsonPropertyName("dimensions_structured")]
    public DimensionsStructured? DimensionsStructured { get; init; }
}
