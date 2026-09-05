using Reveries.Application.BookSeries.Interfaces;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Interfaces.Repositories;

namespace Reveries.Application.BookSeries.Services;

public class SeriesResolver : ISeriesResolver
{
    private readonly ISeriesRepository _series;

    public SeriesResolver(ISeriesRepository series)
    {
        _series = series;
    }

    public async Task<Series> ResolveAsync(Series series, CancellationToken ct = default)
    {
        var existing = await _series.GetByNameAsync(series.Name, ct);
        if (existing is not null)
            return existing;

        await _series.AddAsync(series, ct);

        return series;
    }
}