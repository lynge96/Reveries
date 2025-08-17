using Reveries.Core.Entities;

namespace Reveries.Core.Interfaces.Repositories;

public interface IBookSubjectsRepository
{
    Task SaveBookSubjectsAsync(int bookId, IEnumerable<Subject> subjects);
}