using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Persistence.Repositories;

public interface ISeriesRepository
{
    Task<Series?> GetSeriesByNameAsync(string? seriesName);
    Task<int> CreateSeriesAsync(Series series);
    Task<List<Series>> GetSeriesAsync();
}