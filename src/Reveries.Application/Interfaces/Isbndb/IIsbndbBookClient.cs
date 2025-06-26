using Reveries.Core.DTOs;

namespace Reveries.Application.Interfaces.Isbndb;

public interface IIsbndbBookClient
{
    Task<BookSearchResponseDto?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
}
