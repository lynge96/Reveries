namespace Reveries.Integration.GoogleBooks.Dtos;

public sealed record GoogleVolumeInfoDto
{
    public string? Title { get; init; }
    public string? Subtitle { get; init; }
    public IReadOnlyList<string>? Authors { get; init; }
    public string? Publisher { get; init; }
    public string? PublishedDate { get; init; }
    public string? Description { get; init; }
    public IReadOnlyList<GoogleIndustryIdentifierDto>? IndustryIdentifiers { get; init; }
    public int? PageCount { get; init; }
    public IReadOnlyList<string>? Categories { get; init; }
    public string? Language { get; init; }
    public string? PrintType { get; init; }
    public GoogleImageLinksDto? ImageLinks { get; init; }
    public GoogleDimensionsDto? Dimensions { get; init; }
}