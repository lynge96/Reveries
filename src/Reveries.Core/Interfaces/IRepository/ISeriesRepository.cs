using Reveries.Core.Models;
using Reveries.Core.ValueObjects.DTOs;

namespace Reveries.Core.Interfaces.IRepository;

public interface ISeriesRepository
{
    Task<int> AddAsync(Series series);
    Task<SeriesWithId?> GetByNameAsync(string seriesName);
}