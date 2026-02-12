using Reveries.Core.Models;

namespace Reveries.Application.Interfaces.Services;

public interface IBookPersistenceService
{
    Task<int> SaveBookWithRelationsAsync(Book book, CancellationToken ct = default);
}