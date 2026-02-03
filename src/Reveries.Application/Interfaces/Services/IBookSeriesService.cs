using Reveries.Core.Models;

namespace Reveries.Application.Interfaces.Services;

public interface IBookSeriesService
{
    Task<Series> CreateSeriesAsync(Series series);
    Task<List<Series>> GetSeriesAsync();
}