namespace Reveries.Application.DTOs.IsbndbDtos.Books;

public class BooksQueryResponseDto
{
    public int Total { get; init; }
    
    public IEnumerable<IsbndbBookDto> Books { get; init; } = null!;
}