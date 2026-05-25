using Reveries.Core.Identity;
using Reveries.Core.Models;

namespace Reveries.Application.Books.Interfaces;

public interface IBookPersistenceService
{
    Task<BookId> SaveBookWithRelationsAsync(Book book, CancellationToken ct = default);
}