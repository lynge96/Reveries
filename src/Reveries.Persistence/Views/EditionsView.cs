namespace Reveries.Persistence.Views;

public sealed class EditionsView
{
    // Edition
    public Guid Id { get; init; }
    public Guid WorkId { get; init; }
    public string? Isbn13 { get; init; }
    public string? Isbn10 { get; init; }
    public string? PublicationDate { get; init; }
    public int? PageCount { get; init; }
    public string? Language { get; init; }
    public string? EditionStatement { get; init; }
    public string? Format { get; init; }
    public string? CoverImageUrl { get; init; }
    public string? ImageThumbnailUrl { get; init; }
    public string? SaxoUrl { get; init; }
    public decimal? Msrp { get; init; }
    public decimal? HeightCm { get; init; }
    public decimal? WidthCm { get; init; }
    public decimal? ThicknessCm { get; init; }
    public decimal? WeightG { get; init; }
    public string? DataSource { get; init; }
    public DateTimeOffset? DateCreatedEdition { get; init; }

    // Publisher (null when the edition has no publisher — LEFT JOIN)
    public Guid? PublisherId { get; init; }
    public string? PublisherName { get; init; }
    public DateTimeOffset? DateCreatedPublisher { get; init; }
}