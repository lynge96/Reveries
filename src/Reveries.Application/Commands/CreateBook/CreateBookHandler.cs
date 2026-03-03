using Microsoft.Extensions.Logging;
using Reveries.Application.Commands.Abstractions;
using Reveries.Application.Mappers;
using Reveries.Application.Services;

namespace Reveries.Application.Commands.CreateBook;

public sealed class CreateBookHandler : ICommandHandler<CreateBookCommand, int>
{
    private readonly BookPersistenceService _bookPersistenceService;
    private readonly ILogger<CreateBookHandler> _logger;

    public CreateBookHandler(
        BookPersistenceService bookPersistenceService, 
        ILogger<CreateBookHandler> logger)
    {
        _bookPersistenceService = bookPersistenceService;
        _logger = logger;
    }
    
    public async Task<int> HandleAsync(CreateBookCommand command, CancellationToken ct)
    {
        var book = command.ToDomain();

        _logger.LogDebug(
            "Creating book '{Title}' with ISBN {Isbn}",
            book.Title,
            book.Isbn13?.Value ?? book.Isbn10?.Value);
        
        var bookDbId = await _bookPersistenceService.SaveBookWithRelationsAsync(book, ct);
        
        return bookDbId;
    }
    
}