using Reveries.Domain.Identity;
using Reveries.Domain.Models;

namespace Reveries.Application.Books.Interfaces;

public interface IBookPersistenceService
{
    Task<BookId> SaveBookWithRelationsAsync(Book book, CancellationToken ct = default);
}