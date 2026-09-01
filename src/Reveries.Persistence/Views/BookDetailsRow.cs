namespace Reveries.Persistence.Views;

public sealed class BookDetailsRow
{
    // Edition
    public Guid BookId { get; init; }
    public string? Isbn13 { get; init; }
    public string? Isbn10 { get; init; }
    public string? Language { get; init; }
    public int? PageCount { get; init; }
    public string? PublicationDate { get; init; }
    public string? Format { get; init; }
    public string? EditionStatement { get; init; }
    public string? CoverImageUrl { get; init; }
    public string? ImageThumbnailUrl { get; init; }
    public decimal? HeightCm { get; init; }
    public decimal? WidthCm { get; init; }
    public decimal? ThicknessCm { get; init; }
    public decimal? WeightG { get; init; }

    // Work
    public string Title { get; init; } = null!;
    public string? Subtitle { get; init; }
    public string? Synopsis { get; init; }
    public string? Description { get; init; }

    // Publisher / Series (LEFT JOIN — null when absent)
    public string? PublisherName { get; init; }
    public string? SeriesName { get; init; }
    public int? SeriesNumber { get; init; }

    // Denormalized collections
    public string Authors { get; init; } = "[]";
    public string PrimaryGenres { get; init; } = "[]";
    public string SecondaryGenres { get; init; } = "[]";
    public string[] DeweyCodes { get; init; } = [];
}