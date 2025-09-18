using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Services;

public interface IBookSeriesService
{
    Task<int> CreateSeriesAsync(Series series);
    Task<List<Series>> GetSeriesAsync();
}