using Reveries.Core.Models;

namespace Reveries.Core.Interfaces;

public interface IBookPersistenceService
{
    Task<int> SaveBookWithRelationsAsync(Book book);
}