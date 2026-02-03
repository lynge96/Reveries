using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Core.Interfaces.Persistence.Repositories;

public interface IBookGenresRepository
{
    Task SaveBookGenresAsync(int? bookId, IEnumerable<Genre> genres);
}