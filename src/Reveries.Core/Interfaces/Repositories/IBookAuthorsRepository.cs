using Reveries.Core.Entities;

namespace Reveries.Core.Interfaces.Repositories;

public interface IBookAuthorsRepository
{
    Task SaveBookAuthorsAsync(int bookId, IEnumerable<Author> authors);
}