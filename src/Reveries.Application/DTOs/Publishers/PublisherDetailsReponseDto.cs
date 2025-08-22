using Reveries.Application.DTOs.Books;

namespace Reveries.Application.DTOs.Publishers;

public class PublisherDetailsReponseDto
{
    public string Publisher { get; init; } = string.Empty;
    
    public IEnumerable<BookDto> Books { get; init; } = null!;
}