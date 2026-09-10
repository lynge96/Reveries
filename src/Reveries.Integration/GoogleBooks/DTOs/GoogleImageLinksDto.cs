namespace Reveries.Integration.GoogleBooks.DTOs;

public sealed record GoogleImageLinksDto
{
    public string? Thumbnail { get; init; }
    public string? SmallThumbnail { get; init; }
}