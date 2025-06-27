using Reveries.Core.DTOs.Books;

namespace Reveries.Core.DTOs.Authors;

public class AuthorBooksResponseDto
{
    public string Author { get; init; } = string.Empty;

    public IEnumerable<BookDto> Books { get; init; } = null!;
}