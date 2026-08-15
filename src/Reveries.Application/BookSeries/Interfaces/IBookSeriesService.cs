using Reveries.Domain.Books;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Shared;

namespace Reveries.Application.BookSeries.Interfaces;

public interface IBookSeriesService
{
    Task<BookId> SetSeriesAsync(Isbn? isbn, Series series, int? numberInSeries, CancellationToken ct = default);
}
