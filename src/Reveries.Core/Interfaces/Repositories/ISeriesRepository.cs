using Reveries.Core.Entities;

namespace Reveries.Core.Interfaces.Repositories;

public interface ISeriesRepository
{
    Task<Series?> GetSeriesByNameAsync(string? seriesName);

    Task<int> CreateSeriesAsync(Series series);
}