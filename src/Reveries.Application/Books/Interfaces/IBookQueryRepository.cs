using Reveries.Application.Books.Models;

namespace Reveries.Application.Books.Interfaces;

public interface IBookQueryRepository
{
    Task<IReadOnlyList<Book>> GetAllBooksAsync(CancellationToken ct);
}