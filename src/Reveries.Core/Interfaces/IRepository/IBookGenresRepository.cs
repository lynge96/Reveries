using Reveries.Core.ValueObjects.DTOs;

namespace Reveries.Core.Interfaces.IRepository;

public interface IBookGenresRepository
{
    Task AddAsync(int bookId, IEnumerable<GenreWithId> genres);
}