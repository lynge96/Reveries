using Reveries.Core.Models;

namespace Reveries.Core.Interfaces.IRepository;

public interface IBookAuthorsRepository
{
    Task SaveBookAuthorsAsync(int bookId, IEnumerable<Author> authors);
}