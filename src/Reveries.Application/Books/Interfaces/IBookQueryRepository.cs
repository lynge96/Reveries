using Reveries.Application.Books.Models;

namespace Reveries.Application.Books.Interfaces;

public interface IBookQueryRepository
{
    Task<Book?> GetBookByIdAsync(Guid bookId, CancellationToken ct);
    Task<IReadOnlyList<Book>> GetAllBooksAsync(CancellationToken ct);
}