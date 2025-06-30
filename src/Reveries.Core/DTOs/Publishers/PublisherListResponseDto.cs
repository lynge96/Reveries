namespace Reveries.Core.DTOs.Publishers;

public class PublisherListResponseDto
{
    public int Total { get; init; }

    public IEnumerable<string> Publishers { get; init; } = null!;
}