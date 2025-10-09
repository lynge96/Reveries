using Reveries.Core.Models;

namespace Reveries.Console.Interfaces;

public interface ISaveEntityService
{
    Task SaveBooksAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default);
    Task SaveSeriesAsync(Series series, CancellationToken cancellationToken = default);
}
