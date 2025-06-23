using System.Text.Json.Serialization;

namespace Reveries.Application.DTOs;

public class BookDto
{
    public string? Title { get; set; }
    
    public string? Image { get; set; }
    [JsonPropertyName("image_original")]
    public string? ImageOriginal { get; set; }
    [JsonPropertyName("title_long")]
    public string? TitleLong { get; set; }
    [JsonPropertyName("date_published")]
    public string? DatePublished { get; set; }
    
    public string? Publisher { get; set; }
    
    public string? Synopsis { get; set; }
    
    public List<string>? Subjects { get; set; }
    
    public List<string>? Authors { get; set; }
    
    public string? Isbn13 { get; set; }
    
    public string? Isbn { get; set; }
    
    public string? Isbn10 { get; set; }
    
    public string? Edition { get; set; }
    
    public string? Binding { get; set; }
    
    public int? Msrp { get; set; }
    
    public string? Language { get; set; }
    
    public string? Dimensions { get; set; }
    
    public int? Pages { get; set; }
    [JsonPropertyName("dimensions_structured")]
    public DimensionsStructuredDto? DimensionsStructured { get; set; }
}

public class DimensionsStructuredDto
{
    public DimensionDto Length { get; set; }
    public DimensionDto Width { get; set; }
    public DimensionDto Height { get; set; }
    public DimensionDto Weight { get; set; }
}

public class DimensionDto
{
    public string Unit { get; set; }
    public double Value { get; set; }
}