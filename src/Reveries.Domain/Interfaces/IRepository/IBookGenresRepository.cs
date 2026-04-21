using Reveries.Core.ValueObjects;

namespace Reveries.Core.Interfaces.IRepository;

public interface IBookGenresRepository
{
    Task AddAsync(Guid bookId, IEnumerable<Genre> genres);
}