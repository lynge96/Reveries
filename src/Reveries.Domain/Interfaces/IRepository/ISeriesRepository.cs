using Reveries.Core.Models;

namespace Reveries.Core.Interfaces.IRepository;

public interface ISeriesRepository
{
    Task<Series?> GetOrCreateAsync(Series? series, CancellationToken ct = default);
    Task<Series?> GetByNameAsync(string seriesName);
    Task<List<Series>> GetSeriesAsync();
}