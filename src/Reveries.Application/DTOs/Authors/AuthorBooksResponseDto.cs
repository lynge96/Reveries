using Reveries.Application.DTOs.Books;

namespace Reveries.Application.DTOs.Authors;

public class AuthorBooksResponseDto
{
    public string Author { get; init; } = string.Empty;

    public IEnumerable<IsbndbBookDto> Books { get; init; } = null!;
}