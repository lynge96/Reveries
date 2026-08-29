namespace Reveries.Contracts.Books.Requests;

public record CreateBookRequest
{
    public string? Isbn10 { get; set; }
    public string? Isbn13 { get; set; }
    public required string Title { get; set; }
    public string? Series { get; set; }
    public int? NumberInSeries { get; set; }
    public List<string>? Authors { get; set; }
    public string? Publisher { get; set; }
    public string? Language { get; set; }
    public int? Pages { get; set; }
    public string? PublicationDate { get; set; }
    public string? Synopsis { get; set; }
    public string? Description { get; set; }
    public string? Format { get; set; }
    public string? Edition { get; set; }
    public string? ImageThumbnail { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? HeightCm { get; init; }
    public decimal? WidthCm { get; init; }
    public decimal? ThicknessCm { get; init; }
    public decimal? WeightG { get; init; }
    public List<string>? DeweyDecimals { get; set; }
    public List<string>? PrimaryGenres { get; set; }
    public List<string>? SecondaryGenres { get; set; }
}
