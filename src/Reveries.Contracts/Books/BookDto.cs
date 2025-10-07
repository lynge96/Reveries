namespace Reveries.Contracts.Books;

public class BookDto
{
    public string? Isbn10 { get; set; }
    public string? Isbn13 { get; set; }
    public string? Title { get; set; }
    public int? SeriesNumber { get; set; }
    public string? Series { get; set; }
    public List<string>? Authors { get; set; }
    public string? Publisher { get; set; }
    public string? Language { get; set; }
    public string? LanguageCode { get; set; }
    public int? Pages { get; set; }
    public string? PublishDate { get; set; }
    public string? Synopsis { get; set; }
    public string? Binding { get; set; }
    public string? Edition { get; set; }
    public string? ImageThumbnail { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Msrp { get; set; }
    public bool IsRead { get; set; }
    public decimal? WeightG { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? ThicknessCm { get; set; }
    public List<string>? DeweyDecimal { get; set; }
    public List<string>? Subjects { get; set; }
    public string? DataSource { get; set; }
}