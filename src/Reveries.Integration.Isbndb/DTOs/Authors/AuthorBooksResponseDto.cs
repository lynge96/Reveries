using Reveries.Integration.Isbndb.DTOs.Books;

namespace Reveries.Integration.Isbndb.DTOs.Authors;

public class AuthorBooksResponseDto
{
    public string Author { get; init; } = string.Empty;

    public IEnumerable<IsbndbBookDto> Books { get; init; } = null!;
}