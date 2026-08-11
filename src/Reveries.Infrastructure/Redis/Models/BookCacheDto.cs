namespace Reveries.Infrastructure.Redis.Models;

public sealed record BookCacheDto
{
    public required Guid Id { get; init; }
    public string? Isbn10 { get; init; }
    public string? Isbn13 { get; init; }
    public required string Title { get; init; }
    public SeriesCacheDto? Series { get; init; }
    public int? NumberInSeries { get; init; }
    public List<AuthorCacheDto> Authors { get; init; } = [];
    public PublisherCacheDto? Publisher { get; init; }
    public string? Language { get; init; }
    public int? Pages { get; init; }
    public string? PublicationDate { get; init; }
    public string? Synopsis { get; init; }
    public string? Binding { get; init; }
    public string? Edition { get; init; }
    public string? ImageThumbnailUrl { get; init; }
    public string? CoverImageUrl { get; init; }
    public decimal? Msrp { get; init; }
    public bool IsRead { get; init; }
    public decimal? WeightG { get; init; }
    public decimal? HeightCm { get; init; }
    public decimal? WidthCm { get; init; }
    public decimal? ThicknessCm { get; init; }
    public List<string> DeweyDecimals { get; init; } = [];
    public List<string> Genres { get; init; } = [];
}