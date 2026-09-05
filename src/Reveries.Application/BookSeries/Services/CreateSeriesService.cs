using Reveries.Application.BookSeries.Interfaces;
using Reveries.Application.Common.Exceptions;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.BookSeries;

namespace Reveries.Application.BookSeries.Services;

public class CreateSeriesService : ICreateSeriesService
{
    private readonly ISeriesRepository _series;

    public CreateSeriesService(ISeriesRepository series)
    {
        _series = series;
    }

    public async Task<Series> CreateSeriesAsync(Series series, CancellationToken ct)
    {
        var existingSeries = await _series.GetByNameAsync(series.Name, ct);
        if (existingSeries != null)
        {
            throw new SeriesAlreadyExistsException(series.Name);
        }

        await _series.AddAsync(series, ct);
        return series;
    }

    public async Task<List<Series>> GetSeriesAsync(CancellationToken ct)
    {
        var series = await _series.GetSeriesAsync(ct);
        return series;
    }
}
