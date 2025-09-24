using Reveries.Integration.Isbndb.DTOs.Books;

namespace Reveries.Integration.Isbndb.DTOs.Publishers;

public class PublisherDetailsReponseDto
{
    public string Publisher { get; init; } = string.Empty;
    
    public IEnumerable<IsbndbBookDto> Books { get; init; } = null!;
}