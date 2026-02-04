using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Interfaces;

public interface IGenreRepository
{
    Task<int> AddAsync(GenreEntity genre);
    Task<GenreEntity?> GetByNameAsync(string genreName);
}
