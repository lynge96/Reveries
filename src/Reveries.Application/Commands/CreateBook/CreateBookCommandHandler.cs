using Microsoft.Extensions.Logging;
using Reveries.Application.Interfaces.Cache;
using Reveries.Application.Interfaces.Messaging;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Mappers;
using Reveries.Core.Models;

namespace Reveries.Application.Commands.CreateBook;

public sealed class CreateBookCommandHandler : ICommandHandler<CreateBookCommand, int>
{
    private readonly IBookPersistenceService _bookPersistenceService;
    private readonly IAuthorEnrichmentService _authorEnrichmentService;
    private readonly IBookCacheService _cache;
    private readonly ILogger<CreateBookCommandHandler> _logger;

    public CreateBookCommandHandler(
        IBookPersistenceService bookPersistenceService, 
        IAuthorEnrichmentService authorEnrichmentService, 
        IBookCacheService cache, 
        ILogger<CreateBookCommandHandler> logger)
    {
        _bookPersistenceService = bookPersistenceService;
        _authorEnrichmentService = authorEnrichmentService;
        _cache = cache;
        _logger = logger;
    }
    
    public async Task<int> Handle(CreateBookCommand command, CancellationToken ct)
    {
        var book = command.MapToDomain();

        _logger.LogDebug(
            "Creating book '{Title}' with ISBN {Isbn}",
            book.Title,
            book.Isbn13?.Value ?? book.Isbn10?.Value);

        await _authorEnrichmentService.EnrichAsync(book.Authors, ct);
        
        var bookDbId = await _bookPersistenceService.SaveBookWithRelationsAsync(book);

        await _cache.SetBookByIsbnAsync(book, ct);
        
        return bookDbId;
    }

    public async Task UpdateBooksAsync(List<Book> books, CancellationToken ct)
    {
        foreach (var book in books)
        {
            await _unitOfWork.Books.UpdateBookAsync(book);
            await _cache.RemoveBookByIsbnAsync(book.Isbn13 ?? book.Isbn10, ct);
        }
    }
    
}