using Microsoft.Extensions.Logging;
using Reveries.Application.Authors.Services;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Common.Abstractions;
using Reveries.Application.Common.Exceptions;
using Reveries.Core.Identity;
using Reveries.Core.Models;

namespace Reveries.Application.Books.Services;

public class BookPersistenceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BookPersistenceService> _logger;
    private readonly AuthorEnrichmentService _authorEnrichmentService;
    private readonly IBookCacheService _cache;
    
    public BookPersistenceService(
        IUnitOfWork unitOfWork, 
        ILogger<BookPersistenceService> logger,
        AuthorEnrichmentService authorEnrichmentService,
        IBookCacheService cache)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _authorEnrichmentService = authorEnrichmentService;
        _cache = cache;
    }

    public async Task<BookId> SaveBookWithRelationsAsync(Book book, CancellationToken ct)
    {
        await using var tx = await _unitOfWork.BeginTransactionAsync(ct);
        
        await ValidateBookNotExistsAsync(book, ct);
        
        try
        {
            await _authorEnrichmentService.EnrichAsync(book.Authors, ct);
            
            await SaveBookAsync(book, ct);

            await tx.CommitAsync(ct);

            try
            {
                await _cache.SetBookByIsbnAsync(book, ct);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to cache book with ISBN {Isbn}.", book.Isbn13 ?? book.Isbn10);
            }
            
            return book.Id;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    private async Task ValidateBookNotExistsAsync(Book book, CancellationToken ct)
    {
        var isbn = book.Isbn13 ?? book.Isbn10 ?? null;
        if (isbn == null) return;
        
        var bookExists = await _unitOfWork.Books.BookExistsAsync(isbn, ct);
        
        if (bookExists)
        {
            throw new BookAlreadyExistsException(isbn);
        }
    }

    private async Task SaveBookAsync(Book book, CancellationToken ct)
    {
        // Handle Publisher
        var publisher = await _unitOfWork.Publishers.GetOrCreateAsync(book.Publisher, ct);
        book.SetPublisher(publisher);
                
        // Handle Series
        var series = await _unitOfWork.Series.GetOrCreateAsync(book.Series, ct);
        book.SetSeries(series);
        
        // Insert book
        await _unitOfWork.Books.InsertBookAsync(book, ct);
        
        // Handle Authors and relations
        var authorIds = await _unitOfWork.Authors.GetOrCreateAuthorsAsync(book.Authors, ct);
        await _unitOfWork.BookAuthors.InsertBookAuthorsAsync(book.Id.Value, authorIds, ct);
        
        // Handle Genres and relations
        var genreIds = await _unitOfWork.Genres.GetOrCreateGenresAsync(book.Genres, ct);
        await _unitOfWork.BookGenres.InsertBookGenresAsync(book.Id.Value, genreIds, ct);
        
        // Handle Dewey Decimals and relations
        var deweyDecimalIds = await _unitOfWork.DeweyDecimals.GetOrCreateDeweyDecimalsAsync(book.DeweyDecimals, ct);
        await _unitOfWork.BookDeweyDecimals.InsertBookDeweyDecimalsAsync(book.Id.Value, deweyDecimalIds, ct);
    }
    
}