using Reveries.Core.Models;

namespace Reveries.Core.Interfaces.Persistence.Repositories;

public interface ISeriesRepository
{
    Task<Series?> GetSeriesByNameAsync(string? seriesName);
    Task<int> CreateSeriesAsync(Series series);
    Task<List<Series>> GetSeriesAsync();
}