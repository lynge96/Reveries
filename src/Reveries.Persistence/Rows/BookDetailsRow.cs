namespace Reveries.Persistence.Rows;

public sealed class BookDetailsRow
{
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

    public string Title { get; init; } = null!;
    public string? Subtitle { get; init; }
    public string? Synopsis { get; init; }
    public string? Description { get; init; }

    public string? PublisherName { get; init; }
    public string? SeriesName { get; init; }
    public int? SeriesNumber { get; init; }

    public string Authors { get; init; } = "[]";
    public string PrimaryGenres { get; init; } = "[]";
    public string SecondaryGenres { get; init; } = "[]";
    public string[] DeweyCodes { get; init; } = [];
}