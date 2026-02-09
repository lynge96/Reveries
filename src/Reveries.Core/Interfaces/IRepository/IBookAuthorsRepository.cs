using Reveries.Core.ValueObjects.DTOs;

namespace Reveries.Core.Interfaces.IRepository;

public interface IBookAuthorsRepository
{
    Task AddAsync(int bookId, IEnumerable<AuthorWithId> authors);
}