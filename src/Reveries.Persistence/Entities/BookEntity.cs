namespace Reveries.Persistence.Entities;

public sealed class BookEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Isbn13 { get; set; }
    public string? Isbn10 { get; set; }
    public Guid? PublisherId { get; set; }
    public string? PublicationDate { get; set; }
    public int? PageCount { get; set; }
    public string? Synopsis { get; set; }
    public string? Language { get; set; }
    public string? Edition { get; set; }
    public string? Binding { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageThumbnail { get; set; }
    public decimal? Msrp { get; set; }
    public bool IsRead { get; set; }
    public int? SeriesNumber { get; set; }
    public Guid? SeriesId { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? ThicknessCm { get; set; }
    public decimal? WeightG { get; set; }
    public DateTimeOffset? DateCreated { get; set; }
}