namespace Reveries.Application.Books.Models;

/// <summary>
/// The read/display model for a book: a flat composition of a <c>Work</c> and one of its
/// <c>Edition</c>s, denormalized for querying and returning to the API. This is the read side —
/// writes go through the <c>Work</c> and <c>Edition</c> domain aggregates, never this type.
/// A lookup preview (a book found externally but not yet saved) uses an empty <see cref="BookId"/>.
/// </summary>
public sealed record Book
{
    public required Guid BookId { get; init; }
    public string? Isbn10 { get; init; }
    public string? Isbn13 { get; init; }
    public required string Title { get; init; }
    public string? Subtitle { get; init; }
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
    public string? SaxoUrl { get; init; }
    public decimal? WeightG { get; init; }
    public decimal? HeightCm { get; init; }
    public decimal? WidthCm { get; init; }
    public decimal? ThicknessCm { get; init; }
    public IReadOnlyList<string> DeweyDecimals { get; init; } = [];
    public IReadOnlyList<string> PrimaryGenres { get; init; } = [];
    public IReadOnlyList<string> SecondaryGenres { get; init; } = [];
}