using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Interfaces;

public interface ISeriesRepository
{
    Task<int> AddAsync(SeriesEntity series);
    Task<SeriesEntity?> GetByNameAsync(string seriesName);
    
}