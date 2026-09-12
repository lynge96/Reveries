namespace Reveries.Integration.GoogleBooks.Dtos;

public sealed record GoogleDimensionsDto
{
    public string? Height { get; init; }
    public string? Width { get; init; }
    public string? Thickness { get; init; }
}