using System.Text.Json.Serialization;
using Reveries.Core.Models;

namespace Reveries.Core.DTOs;

public class BookDto
{
    public string? Title { get; init; }
    
    public string? Image { get; init; }
    [JsonPropertyName("image_original")]
    public string? ImageOriginal { get; init; }
    [JsonPropertyName("title_long")]
    public string? TitleLong { get; init; }
    [JsonPropertyName("date_published")]
    public string? DatePublished { get; init; }
    
    public string? Publisher { get; init; }
    
    public string? Synopsis { get; init; }
    
    public List<string>? Subjects { get; init; }
    
    public List<string>? Authors { get; init; }
    
    public string? Isbn13 { get; init; }
    
    public string? Isbn { get; init; }
    
    public string? Isbn10 { get; init; }
    
    public string? Edition { get; init; }
    
    public string? Binding { get; init; }
    
    public int? Msrp { get; init; }
    
    public string? Language { get; init; }
    
    public string? Dimensions { get; init; }
    
    public int? Pages { get; init; }
    [JsonPropertyName("dimensions_structured")]
    public DimensionsStructured? DimensionsStructured { get; init; }
}
