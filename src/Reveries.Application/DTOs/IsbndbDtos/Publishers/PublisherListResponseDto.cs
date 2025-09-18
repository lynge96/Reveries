namespace Reveries.Application.DTOs.IsbndbDtos.Publishers;

public class PublisherListResponseDto
{
    public int Total { get; init; }

    public IEnumerable<string> Publishers { get; init; } = null!;
}