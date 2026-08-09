using Reveries.Core.Models;

namespace Reveries.Core.Interfaces.IRepository;

public interface ISeriesRepository
{
    Task<Series?> GetOrCreateAsync(Series? series, CancellationToken ct = default);
    Task<Series?> GetByNameAsync(Series series, CancellationToken ct = default);
    Task<List<Series>> GetSeriesAsync(CancellationToken ct = default);
}