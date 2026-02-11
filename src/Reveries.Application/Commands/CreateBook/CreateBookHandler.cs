using Microsoft.Extensions.Logging;
using Reveries.Application.Commands.Abstractions;
using Reveries.Application.Interfaces.Cache;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Mappers;

namespace Reveries.Application.Commands.CreateBook;

public sealed class CreateBookHandler : ICommandHandler<CreateBookCommand, int>
{
    private readonly IBookPersistenceService _bookPersistenceService;
    private readonly ILogger<CreateBookHandler> _logger;

    public CreateBookHandler(
        IBookPersistenceService bookPersistenceService, 
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