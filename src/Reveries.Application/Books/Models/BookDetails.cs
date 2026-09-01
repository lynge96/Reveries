namespace Reveries.Application.Books.Models;

public sealed record BookDetails
{
    public required Guid BookId { get; init; }
    public string? Isbn10 { get; init; }
    public string? Isbn13 { get; init; }
    public required string Title { get; init; }
    public string? Subtitle { get; init; }
    public string? Series { get; init; }
    public int? NumberInSeries { get; init; }
    public IReadOnlyList<string> Authors { get; init; } = [];
    public string? Publisher { get; init; }
    public string? Language { get; init; }
    public int? Pages { get; init; }
    public string? PublicationDate { get; init; }
    public string? Synopsis { get; init; }
    public string? Description { get; init; }
    public string? Format { get; init; }
    public string? Edition { get; init; }
    public string? ImageThumbnailUrl { get; init; }
    public string? CoverImageUrl { get; init; }
    public decimal? WeightG { get; init; }
    public decimal? HeightCm { get; init; }
    public decimal? WidthCm { get; init; }
    public decimal? ThicknessCm { get; init; }
    public IReadOnlyList<string> DeweyDecimals { get; init; } = [];
    public IReadOnlyList<string> PrimaryGenres { get; init; } = [];
    public IReadOnlyList<string> SecondaryGenres { get; init; } = [];
}