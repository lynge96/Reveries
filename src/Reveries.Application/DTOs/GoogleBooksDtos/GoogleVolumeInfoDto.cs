
namespace Reveries.Application.DTOs.GoogleBooksDtos;

public class GoogleVolumeInfoDto
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public List<string>? Authors { get; set; }
    public string? Publisher { get; set; }
    public string? PublishedDate { get; set; }
    public string? Description { get; set; }
    public List<GoogleIndustryIdentifierDto>? IndustryIdentifiers { get; set; }
    public int? PageCount { get; set; }
    public List<string>? Categories { get; set; }
    public string? Language { get; set; }
    public string? PrintType { get; set; }
    public ImageLinks? ImageLinks { get; set; }
    public GoogleDimensions? Dimensions { get; set; }
}

public class ImageLinks
{
    public string Thumbnail { get; set; } = string.Empty;
    public string SmallThumbnail { get; set; } = string.Empty;
}

public class GoogleIndustryIdentifierDto
{
    public string Type { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
}

public class GoogleDimensions
{
    public string? Height { get; set; }
    public string? Width { get; set; }
    public string? Thickness { get; set; }
}