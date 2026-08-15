using Reveries.Domain.Books;

namespace Reveries.Application.Books.Interfaces;

public interface IBookPersistenceService
{
    Task<BookId> SaveBookWithRelationsAsync(Book book, CancellationToken ct = default);
}
