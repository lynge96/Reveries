using Reveries.Application.BookSeries.Interfaces;
using Reveries.Application.Common.Abstractions;
using Reveries.Application.Common.Exceptions;
using Reveries.Domain;

namespace Reveries.Application.BookSeries.Services;

public class BookSeriesService : IBookSeriesService
{
    private readonly ITransactionManager _transactionManager;
    private readonly IBookRepository _books;
    private readonly ISeriesRepository _series;

    public BookSeriesService(
        ITransactionManager transactionManager,
        IBookRepository books,
        ISeriesRepository series)
    {
        _transactionManager = transactionManager;
        _books = books;
        _series = series;
    }

    public async Task<BookId> SetSeriesAsync(Isbn? isbn, Series series, int? numberInSeries, CancellationToken ct)
    {
        await using var tx = await _transactionManager.BeginTransactionAsync(ct);

        var existingBook = await _books.GetBookByIsbnAsync(isbn, ct: ct);
        if (existingBook == null)
            throw new NotFoundException($"Book with ISBN '{isbn}' was not found.");

        var existingSeries = await _series.GetByNameAsync(series, ct);

        if (existingSeries != null)
        {
            existingBook.SetSeries(existingSeries, numberInSeries);
            await _books.UpdateBookSeriesAsync(existingBook, existingSeries.Id.Value, ct);
        }
        else
        {
            existingBook.SetSeries(series, numberInSeries);
            var createdSeries = await _series.GetOrCreateAsync(series, ct: ct);
            await _books.UpdateBookSeriesAsync(existingBook, createdSeries!.Id.Value, ct);
        }

        await tx.CommitAsync(ct);

        return existingBook.Id;
    }
}
