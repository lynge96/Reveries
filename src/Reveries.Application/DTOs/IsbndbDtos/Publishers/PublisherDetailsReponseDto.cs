using Reveries.Application.DTOs.IsbndbDtos.Books;

namespace Reveries.Application.DTOs.IsbndbDtos.Publishers;

public class PublisherDetailsReponseDto
{
    public string Publisher { get; init; } = string.Empty;
    
    public IEnumerable<IsbndbBookDto> Books { get; init; } = null!;
}