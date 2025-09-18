using Reveries.Application.DTOs.IsbndbDtos.Books;

namespace Reveries.Application.DTOs.IsbndbDtos.Authors;

public class AuthorBooksResponseDto
{
    public string Author { get; init; } = string.Empty;

    public IEnumerable<IsbndbBookDto> Books { get; init; } = null!;
}