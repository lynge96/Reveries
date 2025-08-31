namespace Reveries.Application.DTOs.Books;

public class BooksQueryResponseDto
{
    public int Total { get; init; }
    
    public IEnumerable<IsbndbBookDto> Books { get; init; } = null!;
}