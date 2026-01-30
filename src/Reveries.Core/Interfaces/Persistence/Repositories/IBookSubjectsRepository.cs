using Reveries.Core.Models;

namespace Reveries.Core.Interfaces.Persistence.Repositories;

public interface IBookSubjectsRepository
{
    Task SaveBookSubjectsAsync(int? bookId, IEnumerable<Subject> subjects);
}