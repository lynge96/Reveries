using Microsoft.Extensions.Logging;
using Reveries.Application.Interfaces.Cache;
using Reveries.Application.Interfaces.Messaging;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Mappers;

namespace Reveries.Application.Commands.CreateBook;

public sealed class CreateBookCommandHandler : ICommandHandler<CreateBookCommand, int>
{
    private readonly IBookPersistenceService _bookPersistenceService;
    private readonly IBookCacheService _cache;
    private readonly ILogger<CreateBookCommandHandler> _logger;

    public CreateBookCommandHandler(
        IBookPersistenceService bookPersistenceService, 
        IBookCacheService cache, 
        ILogger<CreateBookCommandHandler> logger)
    {
        _bookPersistenceService = bookPersistenceService;
        _cache = cache;
        _logger = logger;
    }
    
    public async Task<int> Handle(CreateBookCommand command, CancellationToken ct)
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