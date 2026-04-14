using Reveries.Core.ValueObjects;
using Reveries.Core.ValueObjects.DTOs;

namespace Reveries.Core.Interfaces.IRepository;

public interface IGenreRepository
{
    Task<int> AddAsync(Genre genre);
    Task<GenreWithId?> GetByNameAsync(string genreName);
    Task<IReadOnlyList<GenreWithId>> GetByNamesAsync(IEnumerable<string> names);
}
