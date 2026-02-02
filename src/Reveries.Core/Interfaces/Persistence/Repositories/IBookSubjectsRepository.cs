using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Core.Interfaces.Persistence.Repositories;

public interface IBookSubjectsRepository
{
    Task SaveBookSubjectsAsync(int? bookId, IEnumerable<Genre> subjects);
}