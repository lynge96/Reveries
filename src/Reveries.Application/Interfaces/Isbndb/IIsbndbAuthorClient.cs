using Reveries.Core.DTOs;
using Reveries.Core.DTOs.Authors;

namespace Reveries.Application.Interfaces.Isbndb;

public interface IIsbndbAuthorClient
{
    Task<AuthorSearchResponseDto?> GetAuthorsByNameAsync(string authorName, CancellationToken cancellationToken = default);
    
    Task<AuthorBooksResponseDto?> GetBooksByAuthorAsync(string authorName, CancellationToken cancellationToken = default);
}