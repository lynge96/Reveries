using Reveries.Domain.Enums;

namespace Reveries.Application.Books.Caching;

/// <summary>
/// Flat, System.Text.Json-serializable snapshot of a <see cref="Models.BookCandidate"/> for the
/// external-lookup cache. The candidate's value objects (<c>Isbn</c>, <c>Cover</c>, <c>Language</c>,
/// <c>BookDimensions</c>) are flattened to scalars because they have private constructors and cannot
/// be deserialized directly.
/// </summary>
public sealed record CachedBook
{
    public string? Isbn13 { get; init; }
    public string? Isbn10 { get; init; }
    public required string Title { get; init; }
    public string? Subtitle { get; init; }
    public IReadOnlyList<string> Authors { get; init; } = [];
    public string? Publisher { get; init; }
    public IReadOnlyList<string> PrimaryGenres { get; init; } = [];
    public IReadOnlyList<string> SecondaryGenres { get; init; } = [];
    public IReadOnlyList<string> DeweyDecimals { get; init; } = [];
    public string? Synopsis { get; init; }
    public string? Description { get; init; }
    public int? Pages { get; init; }
    public string? PublicationDate { get; init; }
    public string? LanguageCode { get; init; }
    public BookFormat Format { get; init; }
    public string? EditionStatement { get; init; }
    public string? CoverUrl { get; init; }
    public string? CoverThumbnailUrl { get; init; }
    public decimal? HeightCm { get; init; }
    public decimal? WidthCm { get; init; }
    public decimal? ThicknessCm { get; init; }
    public decimal? WeightG { get; init; }
}
