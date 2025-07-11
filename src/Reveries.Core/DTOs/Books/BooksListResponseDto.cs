namespace Reveries.Core.DTOs.Books;

public class BooksListResponseDto
{
    public int Total { get; init; }
    
    public string Requested { get; init; } = string.Empty;

    public IEnumerable<BookDto> Data { get; init; } = null!;
}
