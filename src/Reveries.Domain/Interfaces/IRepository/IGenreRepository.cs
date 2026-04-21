using Reveries.Core.ValueObjects;

namespace Reveries.Core.Interfaces.IRepository;

public interface IGenreRepository
{
    Task<int> AddAsync(Genre genre);
    Task<Genre?> GetByNameAsync(string genreName);
    Task<IReadOnlyList<Genre>> GetByNamesAsync(IEnumerable<string> names);
}
