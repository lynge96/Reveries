namespace Reveries.Contracts.DTOs;

public record BookDto
{
    public string? Isbn10 { get; set; }
    public string? Isbn13 { get; set; }
    public string? Title { get; set; }
    public string? Series { get; set; }
    public int? NumberInSeries { get; set; }
    public List<string>? Authors { get; set; }
    public string? Publisher { get; set; }
    public string? Language { get; set; }
    public int? Pages { get; set; }
    public string? PublicationDate { get; set; }
    public string? Synopsis { get; set; }
    public string? Binding { get; set; }
    public string? Edition { get; set; }
    public string? ImageThumbnail { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Msrp { get; set; }
    public bool IsRead { get; set; }
    public DimensionsDto? Dimensions { get; set; }
    public List<string>? DeweyDecimal { get; set; }
    public List<string>? Subjects { get; set; }
    public string? DataSource { get; set; }
}
