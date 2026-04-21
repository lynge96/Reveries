using Reveries.Core.Models;

namespace Reveries.Core.Interfaces.IRepository;

public interface ISeriesRepository
{
    Task<Guid> AddAsync(Series series);
    Task<Series?> GetByNameAsync(string seriesName);
    Task<List<Series>> GetSeriesAsync();
}