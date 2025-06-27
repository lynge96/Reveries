namespace Reveries.Core.DTOs.Authors;

public class AuthorSearchResponseDto
{
    public int Total { get; init; }

    public IEnumerable<string> Authors { get; init; } = null!;
}