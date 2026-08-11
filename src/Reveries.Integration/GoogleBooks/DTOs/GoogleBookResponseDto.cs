namespace Reveries.Integration.GoogleBooks.DTOs;

public class GoogleBookResponseDto
{
    public int TotalItems { get; set; }
    public List<GoogleBookItemDto>? Items { get; set; }
}