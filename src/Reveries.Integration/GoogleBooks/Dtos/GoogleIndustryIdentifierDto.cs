namespace Reveries.Integration.GoogleBooks.Dtos;

public sealed record GoogleIndustryIdentifierDto
{
    public string? Type { get; init; }
    public string? Identifier { get; init; }
}