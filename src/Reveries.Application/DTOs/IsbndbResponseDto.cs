namespace Reveries.Application.DTOs;

public class IsbndbResponseDto
{
    public int Total { get; set; }
    public List<BookDto> Books { get; set; }
}