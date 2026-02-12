namespace Reveries.Infrastructure.Postgresql.Entities;

public sealed class BookEntity
{
    public int Id { get; set; }
    public Guid BookDomainId { get; set; }
    public string Title { get; set; } = null!;
    public string? Isbn13 { get; set; }
    public string? Isbn10 { get; set; }
    public int? PublisherId { get; set; }
    public string? PublicationDate { get; set; }
    public int? PageCount { get; set; }
    public string? Synopsis { get; set; }
    public string? Language { get; set; }
    public string? Edition { get; set; }
    public string? Binding { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }
    public decimal? Msrp { get; set; }
    public bool IsRead { get; set; }
    public int? SeriesNumber { get; set; }
    public int? SeriesId { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? ThicknessCm { get; set; }
    public decimal? WeightG { get; set; }
    public DateTimeOffset? DateCreatedBook { get; set; }
}