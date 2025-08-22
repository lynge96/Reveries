namespace Reveries.Application.DTOs.Books;

public class BooksListResponseDto
{
    public int Total { get; init; }
    
    public int Requested { get; init; }

    public IEnumerable<BookDto> Data { get; init; } = null!;
}
