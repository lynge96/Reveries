using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Mappers;
using Reveries.Application.Books.Services;
using Reveries.Core.Identity;

namespace Reveries.Application.Books.Commands.CreateBook;

public sealed class CreateBookHandler : IQueryHandler<CreateBookCommand, BookId>
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
    
    public async ValueTask<BookId> Handle(CreateBookCommand command, CancellationToken ct)
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