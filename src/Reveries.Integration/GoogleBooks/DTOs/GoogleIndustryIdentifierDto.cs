namespace Reveries.Integration.GoogleBooks.DTOs;

public sealed record GoogleIndustryIdentifierDto
{
    public string? Type { get; init; }
    public string? Identifier { get; init; }
}