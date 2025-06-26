namespace Reveries.Core.DTOs;

public class BooksQueryResponseDto
{
    public int TotalBooks { get; init; }
    
    public IEnumerable<BookDto> Books { get; init; } = null!;
}