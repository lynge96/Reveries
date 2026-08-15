using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Mappers;
using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Commands.CreateBook;

public sealed class CreateBookHandler : IQueryHandler<CreateBookCommand, EditionId>
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
        var (work, edition) = command.ToWorkAndEdition();

        _logger.LogDebug(
            "Creating work '{Title}' with edition ISBN {Isbn}",
            work.Title,
            edition.Isbn13?.Value ?? edition.Isbn10?.Value);

        var editionId = await _workPersistenceService.SaveWorkWithEditionAsync(work, edition, ct);

        return editionId;
    }
}