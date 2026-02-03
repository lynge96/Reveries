using Reveries.Core.ValueObjects;

namespace Reveries.Core.Interfaces.Persistence.Repositories;

public interface IGenreRepository
{
    Task<Genre?> GetGenreByNameAsync(string genre);
    Task<Genre> CreateGenreAsync(Genre genre);
}
