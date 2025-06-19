using Reveries.Application.DTOs;

namespace Reveries.Application.Interfaces;

public interface IIsbndbClient
{
    Task<BookDto?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
}
