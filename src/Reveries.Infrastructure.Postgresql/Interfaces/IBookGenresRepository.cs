using Reveries.Core.ValueObjects;

namespace Reveries.Infrastructure.Postgresql.Interfaces;

public interface IBookGenresRepository
{
    Task SaveBookGenresAsync(int? bookId, IEnumerable<Genre> genres);
}