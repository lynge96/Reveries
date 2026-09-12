using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Mappers;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Commands.CreateBook;


public sealed class CreateBookHandler : ICommandHandler<CreateBookCommand, EditionId>
{
    private readonly IWorkPersistenceService _workPersistenceService;
    private readonly ILogger<CreateBookHandler> _logger;

    public CreateBookHandler(
        IWorkPersistenceService workPersistenceService,
        ILogger<CreateBookHandler> logger)
    {
        _workPersistenceService = workPersistenceService;
        _logger = logger;
    }

    public async ValueTask<EditionId> Handle(CreateBookCommand command, CancellationToken ct)
    {
        var candidate = command.ToCandidate();
        var series = string.IsNullOrWhiteSpace(command.Series) ? null : Series.Create(command.Series);

        _logger.LogDebug(
            "Creating book '{Title}' with ISBN {Isbn}",
            candidate.Title,
            candidate.Isbn?.Value13);

        var editionId = await _workPersistenceService.SaveBookAsync(candidate, series, command.NumberInSeries, ct);

        return editionId;
    }
}