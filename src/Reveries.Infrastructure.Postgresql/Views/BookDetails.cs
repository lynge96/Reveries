namespace Reveries.Infrastructure.Postgresql.Views;

public sealed class BookDetails
{
    // Book
    public Guid BookId { get; init; }
    public string Title { get; init; } = null!;
    public string? Isbn13 { get; init; }
    public string? Isbn10 { get; init; }
    public string? PublicationDate { get; init; }
    public int? PageCount { get; init; }
    public string? Synopsis { get; init; }
    public string? Language { get; init; }
    public string? Edition { get; init; }
    public string? Binding { get; init; }
    public string? CoverImageUrl { get; init; }
    public string? ImageThumbnailUrl { get; init; }
    public decimal? Msrp { get; init; }
    public bool IsRead { get; init; }
    public int? SeriesNumber { get; init; }
    public decimal? HeightCm { get; init; }
    public decimal? WidthCm { get; init; }
    public decimal? ThicknessCm { get; init; }
    public decimal? WeightG { get; init; }
    public DateTimeOffset? DateCreatedBook { get; init; }
    
    // Publisher
    public Guid PublisherId { get; init; }
    public required string PublisherName { get; init; }
    public DateTimeOffset DateCreatedPublisher { get; init; }

    // Series
    public Guid SeriesId { get; init; }
    public required string SeriesName { get; init; }
    public DateTimeOffset DateCreatedSeries { get; init; }

    // JSON fields
    public string Genres { get; init; } = "[]";
    public string Authors { get; init; } = "[]";

    // text[]
    public string[] DeweyCodes { get; init; } = [];
}