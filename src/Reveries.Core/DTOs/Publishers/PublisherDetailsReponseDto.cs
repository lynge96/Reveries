using Reveries.Core.DTOs.Books;

namespace Reveries.Core.DTOs.Publishers;

public class PublisherDetailsReponseDto
{
    public string Publisher { get; init; } = string.Empty;
    
    public IEnumerable<BookDto> Books { get; init; } = null!;
}