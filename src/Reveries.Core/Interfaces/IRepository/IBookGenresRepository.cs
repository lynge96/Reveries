using Reveries.Core.ValueObjects;

namespace Reveries.Core.Interfaces.IRepository;

public interface IBookGenresRepository
{
    Task SaveBookGenresAsync(int bookId, IEnumerable<Genre> genres);
}