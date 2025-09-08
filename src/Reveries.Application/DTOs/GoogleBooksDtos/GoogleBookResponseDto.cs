namespace Reveries.Application.DTOs.GoogleBooksDtos;

public class GoogleBookResponseDto
{
    public string Kind { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public List<GoogleBookItemDto>? Items { get; set; }
}