namespace Reveries.Core.DTOs;

public class AuthorSearchResponseDto
{
    public int Total { get; init; }
    public List<string> Authors { get; init; } = new();
}