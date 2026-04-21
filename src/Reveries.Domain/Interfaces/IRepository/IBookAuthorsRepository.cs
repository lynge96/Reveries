using Reveries.Core.Models;

namespace Reveries.Core.Interfaces.IRepository;

public interface IBookAuthorsRepository
{
    Task AddAsync(Guid bookId, IEnumerable<Author> authors);
}