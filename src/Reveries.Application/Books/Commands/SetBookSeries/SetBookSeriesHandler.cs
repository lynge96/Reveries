using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.BookSeries.Services;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Commands.SetBookSeries;

public sealed class SetBookSeriesHandler : IQueryHandler<SetBookSeriesCommand, WorkId>
{
    private readonly BookSeriesService _bookSeriesService;
    private readonly ILogger<SetBookSeriesHandler> _logger;

    public SetBookSeriesHandler(
        BookSeriesService bookSeriesService,
        ILogger<SetBookSeriesHandler> logger)
    {
        _bookSeriesService = bookSeriesService;
        _logger = logger;
    }

    public async ValueTask<WorkId> Handle(SetBookSeriesCommand command, CancellationToken ct)
    {
        var series = Series.Create(command.SeriesName);

        var workId = await _bookSeriesService.SetSeriesAsync(command.Isbn, series, command.NumberInSeries, ct);

        _logger.LogDebug(
            "Setting series '{SeriesName}' #{NumberInSeries}, for book with ISBN '{Isbn}'",
            series.Name,
            command.NumberInSeries,
            command.Isbn?.Value13);

        return workId;
    }
}
