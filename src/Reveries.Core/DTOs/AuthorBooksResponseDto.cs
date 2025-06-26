namespace Reveries.Core.DTOs;

public class AuthorBooksResponseDto
{
    public string Author { get; init; } = string.Empty;

    public List<BookDto> Books { get; init; } = new();
}