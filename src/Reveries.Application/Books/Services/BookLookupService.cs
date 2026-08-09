using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Abstractions;
using Reveries.Application.Common.Exceptions;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Services;

public class BookLookupService : IBookLookupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookMergerService _bookMergerService;
    private readonly IBookCacheService _bookCacheService;
    private readonly ILogger<BookLookupService> _logger;
    private readonly IIsbndbBookSearch _isbnDbClient;
    private readonly IGoogleBookSearch _googleBooksClient;

    public BookLookupService(
        IIsbndbBookSearch isbnDbClient, 
        IGoogleBookSearch googleBooksClient,
        IUnitOfWork unitOfWork, 
        IBookMergerService bookMergerService, 
        IBookCacheService bookCacheService, 
        ILogger<BookLookupService> logger)
    {
        _isbnDbClient = isbnDbClient;
        _googleBooksClient = googleBooksClient;
        _unitOfWork = unitOfWork;
        _bookMergerService = bookMergerService;
        _bookCacheService = bookCacheService;
        _logger = logger;
    }
    
    public async Task<BookLookupResult<Isbn>> LookupByIsbnAsync(Isbn isbn, CancellationToken ct)
    {
        var result = await LookupByIsbnsAsync([isbn], ct);
        return result;
    }

    public async Task<BookLookupResult<Isbn>> LookupByIsbnsAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return BookLookupResult<Isbn>.Empty;
        
        var results = await Task.WhenAll(
            TryLookupFromIsbnDbAsync(isbns, ct),
            TryLookupFromGoogleBooksAsync(isbns, ct)
        );
        
        var isbndbBooks = results[0];
        var googleBooks = results[1];

        if (googleBooks is null && isbndbBooks is null)
        {
            _logger.LogWarning(
                "All external sources failed for ISBNs: {Isbns}", 
                string.Join(", ", isbns.Select(i => i.Value)));
            return new BookLookupResult<Isbn>([], isbns.ToList());
        }
        
        var mergedBooks = _bookMergerService.AggregateBooksByIsbnsAsync(isbns, isbndbBooks, googleBooks);
        
        var foundIsbnKeys = mergedBooks
            .Select(b => b.Isbn13?.Value ?? b.Isbn10?.Value)
            .Where(k => k is not null)
            .ToHashSet();

        var missingIsbns = isbns
            .Where(isbn => !foundIsbnKeys.Contains(isbn.Value))
            .ToList();

        _logger.LogInformation(
            "ISBN lookup completed. Requested: {Requested}, Found: {Found}, NotFound: {NotFound}, Sources: ISBNDB={IsbnDbCount}, Google={GoogleCount}",
            isbns.Count,
            mergedBooks.Count,
            missingIsbns.Count,
            isbndbBooks?.Count ?? 0,
            googleBooks?.Count ?? 0);

        return new BookLookupResult<Isbn>(mergedBooks, missingIsbns);
    }

    public async Task<BookLookupResult<Title>> LookupByTitleAsync(Title title, CancellationToken ct)
    {
        var result = await LookupByTitlesAsync([title], ct);
        return result;
    }

    public async Task<BookLookupResult<Title>> LookupByTitlesAsync(IReadOnlyList<Title> titles, CancellationToken ct)
    {
        if (titles.Count == 0)
            return BookLookupResult<Title>.Empty;
        
        var results = await Task.WhenAll(
            TryLookupFromIsbnDbAsync(titles, ct),
            TryLookupFromGoogleBooksAsync(titles, ct)
        );
        
        var isbndbBooks = results[0];
        var googleBooks = results[1];
        
        if (googleBooks is null && isbndbBooks is null)
        {
            _logger.LogWarning(
                "All external sources failed for ISBNs: {Isbns}", 
                string.Join(", ", titles.Select(t => t)));
            return new BookLookupResult<Title>([], titles);
        }
        
        var mergedBooks = _bookMergerService.AggregateBooksByTitlesAsync(titles, isbndbBooks, googleBooks);
        
        var foundTitles = mergedBooks
            .Select(b => b.Title)
            .ToHashSet();
        
        var missingTitles = titles
            .Where(t => !foundTitles.Contains(t))
            .ToList();
        
        _logger.LogInformation(
            "Titles lookup completed. Requested: {Requested}, Found: {Found}, NotFound: {NotFound}, Sources: ISBNDB={IsbnDbCount}, Google={GoogleCount}",
            titles.Count,
            mergedBooks.Count,
            missingTitles.Count,
            isbndbBooks?.Count ?? 0,
            googleBooks?.Count ?? 0);
        
        return new BookLookupResult<Title>(mergedBooks, missingTitles);   
    }
    
    public async Task<List<Book>> GetAllBooksAsync(CancellationToken ct)
    {
        var databaseBooks = await _unitOfWork.Books.GetAllBooksAsync(ct);
        if (databaseBooks.Count == 0)
            return [];
        
        await _bookCacheService.SetBooksByIsbnsAsync(databaseBooks, ct);
        
        _logger.LogInformation("Book lookup by all books completed. DB: {DbCount}.", databaseBooks.Count);
        return databaseBooks;
    }
    
    public async Task<Book?> FindBookById(Guid id, CancellationToken ct)
    {
        var databaseBook = await _unitOfWork.Books.GetBookByIdAsync(id, ct);
        
        _logger.LogInformation("Book lookup by Id completed. Id: {BookId}.", id);
        return databaseBook;
    }

    public async Task<bool> BookExistsAsync(Isbn isbn, CancellationToken ct)
    {
        return await _unitOfWork.Books.BookExistsAsync(isbn, ct);
    }
    
    private async Task<IReadOnlyList<Book>?> TryLookupFromIsbnDbAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct)
    {
        try
        {
            return await _isbnDbClient.GetBooksByIsbnsAsync(isbns, ct);
        }
        catch (ExternalDependencyException ex)
        {
            _logger.LogWarning(ex, "IsbnDb lookup failed, returning all as not found");
            return null;
        }
    }

    private async Task<IReadOnlyList<Book>?> TryLookupFromIsbnDbAsync(IReadOnlyList<Title> titles,
        CancellationToken ct)
    {
        try
        {
            return await _isbnDbClient.GetBooksByTitlesAsync(titles, null, ct);
        }
        catch (ExternalDependencyException ex)
        {
            _logger.LogWarning(ex, "IsbnDb lookup failed, returning all as not found");
            return null;
        }
    }

    private async Task<IReadOnlyList<Book>?> TryLookupFromGoogleBooksAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct)
    {
        try
        {
            return await _googleBooksClient.GetBooksByIsbnsAsync(isbns, ct);
        }
        catch (ExternalDependencyException ex)
        {
            _logger.LogWarning(ex, "GoogleBooks lookup failed, returning all as not found");
            return null;
        }
    }

    private async Task<IReadOnlyList<Book>?> TryLookupFromGoogleBooksAsync(IReadOnlyList<Title> titles,
        CancellationToken ct)
    {
        try
        {
            return await _googleBooksClient.GetBooksByTitlesAsync(titles, ct);
        }
        catch (ExternalDependencyException ex)
        {
            _logger.LogWarning(ex, "GoogleBooks lookup failed, returning all as not found");
            return null;
        }
    }
}