using Reveries.Domain.BookSeries;

namespace Reveries.Application.BookSeries.Interfaces;

public interface ISeriesResolver
{
    Task<Series> ResolveAsync(Series series, CancellationToken ct = default);
}