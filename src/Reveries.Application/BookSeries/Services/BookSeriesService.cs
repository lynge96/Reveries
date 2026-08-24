using Reveries.Application.BookSeries.Interfaces;
using Reveries.Application.Common.Abstractions;
using Reveries.Application.Common.Exceptions;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Application.BookSeries.Services;

public class BookSeriesService : IBookSeriesService
{
    private readonly ITransactionManager _transactionManager;
    private readonly IEditionRepository _editions;
    private readonly IWorkRepository _works;
    private readonly ISeriesRepository _series;

    public BookSeriesService(
        ITransactionManager transactionManager,
        IEditionRepository editions,
        IWorkRepository works,
        ISeriesRepository series)
    {
        _transactionManager = transactionManager;
        _editions = editions;
        _works = works;
        _series = series;
    }

    public async Task<WorkId> SetSeriesAsync(Isbn? isbn, Series series, int? numberInSeries, CancellationToken ct)
    {
        await using var tx = await _transactionManager.BeginTransactionAsync(ct);

        var edition = await _editions.GetEditionByIsbnAsync(isbn, ct: ct);
        if (edition is null)
            throw new NotFoundException($"Edition with ISBN '{isbn}' was not found.");

        var work = await _works.GetWorkByIdAsync(edition.WorkId, ct);
        if (work is null)
            throw new NotFoundException($"Work for ISBN '{isbn}' was not found.");

        var existingSeries = await _series.GetByNameAsync(series, ct);

        if (existingSeries != null)
        {
            work.SetSeries(existingSeries, numberInSeries);
            await _works.UpdateWorkSeriesAsync(work, existingSeries.Id, ct);
        }
        else
        {
            work.SetSeries(series, numberInSeries);
            var createdSeries = await _series.GetOrCreateAsync(series, ct: ct);
            await _works.UpdateWorkSeriesAsync(work, createdSeries!.Id, ct);
        }

        await tx.CommitAsync(ct);

        return work.Id;
    }
}
