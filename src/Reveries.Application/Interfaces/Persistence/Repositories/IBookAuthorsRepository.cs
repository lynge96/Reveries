using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Persistence.Repositories;

public interface IBookAuthorsRepository
{
    Task SaveBookAuthorsAsync(int bookId, IEnumerable<Author> authors);
}