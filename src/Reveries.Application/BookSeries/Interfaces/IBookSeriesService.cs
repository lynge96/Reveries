using Reveries.Domain.Identity;
using Reveries.Domain.Models;
using Reveries.Domain.ValueObjects;

namespace Reveries.Application.BookSeries.Interfaces;

public interface IBookSeriesService
{
    Task<BookId> SetSeriesAsync(Isbn? isbn, Series series, int? numberInSeries, CancellationToken ct = default);
}