namespace Reveries.Core.Interfaces.Repositories;

public interface ISeriesRepository
{
    Task<int> GetOrCreateSeriesAsync(string seriesName);
}