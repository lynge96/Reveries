using Reveries.Domain;

namespace Reveries.Application.Books.Interfaces;

public interface IBookPersistenceService
{
    Task<BookId> SaveBookWithRelationsAsync(Book book, CancellationToken ct = default);
}
