using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Interfaces;

public interface IBookGenresRepository
{
    Task SaveBookGenresAsync(int bookId, IEnumerable<GenreEntity> genres);
}