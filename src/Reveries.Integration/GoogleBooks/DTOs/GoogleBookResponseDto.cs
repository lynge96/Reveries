namespace Reveries.Integration.GoogleBooks.DTOs;

public sealed record GoogleBookResponseDto
{
    public int TotalItems { get; init; }
    public IReadOnlyList<GoogleBookItemDto>? Items { get; init; }
}