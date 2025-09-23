using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Persistence.Repositories;

public interface IBookSubjectsRepository
{
    Task SaveBookSubjectsAsync(int bookId, IEnumerable<Subject> subjects);
}