using Reveries.Core.Identity;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.BookSeries.Interfaces;

public interface IBookSeriesService
{
    Task<BookId> SetSeriesAsync(Isbn? isbn, Series series, int? numberInSeries, CancellationToken ct = default);
}