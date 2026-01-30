using Reveries.Core.Models;

namespace Reveries.Core.Interfaces.Persistence.Repositories;

public interface IBookAuthorsRepository
{
    Task SaveBookAuthorsAsync(int? bookId, IEnumerable<Author> authors);
}