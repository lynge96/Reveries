using Reveries.Domain.BookSeries;
using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Application.BookSeries.Interfaces;

public interface IBookSeriesService
{
    Task<WorkId> SetSeriesAsync(Isbn? isbn, Series series, int? numberInSeries, CancellationToken ct = default);
}
