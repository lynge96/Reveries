namespace Reveries.Integration.GoogleBooks.Dtos;

public sealed record GoogleImageLinksDto
{
    public string? Thumbnail { get; init; }
    public string? SmallThumbnail { get; init; }
}