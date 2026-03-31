using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Mappers;
using Reveries.Application.Books.Services;

namespace Reveries.Application.Books.Commands.CreateBook;

public sealed class CreateBookHandler : IQueryHandler<CreateBookCommand, int>
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
    
    public async ValueTask<int> Handle(CreateBookCommand command, CancellationToken ct)
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