using Reveries.Core.Models;

namespace Reveries.Application.BookSeries.Interfaces;

public interface ICreateSeriesService
{
    Task<Series> CreateSeriesAsync(Series series, CancellationToken ct = default);
    Task<List<Series>> GetSeriesAsync(CancellationToken ct = default);
}