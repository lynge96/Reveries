namespace Reveries.Core.DTOs.Books;

public class BooksQueryResponseDto
{
    public int Total { get; init; }
    
    public IEnumerable<BookDto> Books { get; init; } = null!;
}