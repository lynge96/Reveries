using Reveries.Application.Books.Models;

namespace Reveries.Application.Books.Interfaces;

public interface IBookQueryRepository
{
    Task<BookDetails?> GetBookByIdAsync(Guid bookId, CancellationToken ct);
    Task<IReadOnlyList<BookDetails>> GetAllBooksAsync(CancellationToken ct);
}