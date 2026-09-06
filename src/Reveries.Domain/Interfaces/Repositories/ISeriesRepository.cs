using Reveries.Domain.BookSeries;

namespace Reveries.Domain.Interfaces.Repositories;

public interface ISeriesRepository
{
    Task<Series?> GetByNameAsync(string name, CancellationToken ct = default);
    Task AddAsync(Series series, CancellationToken ct = default);
    Task<List<Series>> GetSeriesAsync(CancellationToken ct = default);
}