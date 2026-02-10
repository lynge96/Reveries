using Microsoft.Extensions.Logging;
using Reveries.Application.Extensions;
using Reveries.Application.Interfaces.Cache;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Interfaces;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Services;

public class BookLookupService : IBookLookupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookEnrichmentService _bookEnrichmentService;
    private readonly IIsbndbAuthorService _isbndbAuthorService;
    private readonly IIsbndbPublisherService _isbndbPublisherService;
    private readonly IBookCacheService _bookCacheService;
    private readonly ILogger<BookLookupService> _logger;

    public BookLookupService(IUnitOfWork unitOfWork, IBookEnrichmentService bookEnrichmentService, IIsbndbAuthorService isbndbAuthorService, IIsbndbPublisherService isbndbPublisherService, IBookCacheService bookCacheService, ILogger<BookLookupService> logger)
    {
        _unitOfWork = unitOfWork;
        _bookEnrichmentService = bookEnrichmentService;
        _isbndbAuthorService = isbndbAuthorService;
        _isbndbPublisherService = isbndbPublisherService;
        _bookCacheService = bookCacheService;
        _logger = logger;
    }
    
    public async Task<Book> FindBookByIsbnAsync(Isbn isbns, CancellationToken ct)
    {
        var books = await FindBooksByIsbnAsync([isbns], ct);
        var book = books.First();
        
        return book;
    }

    public async Task<List<Book>> FindBooksByIsbnAsync(List<Isbn> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return [];
        
        var cacheResult = await GetFromCacheAsync(isbns, ct);
        var dbResult = await GetFromDatabaseAsync(cacheResult.Missing, ct);
        var apiResult = await GetFromApiAsync(dbResult.Missing, ct);

        var allBooks = cacheResult.Found
            .Concat(dbResult.Found)
            .Concat(apiResult.Found)
            .ToList();

        var booksToCache = dbResult.Found.Concat(apiResult.Found).ToList();
        if (booksToCache.Count != 0)
        {
            await _bookCacheService.SetBooksByIsbnsAsync(booksToCache, ct);
        }
        
        _logger.LogInformation(
            "Book lookup completed. Requested {Requested}. Cache {Cache}, DB {Db}, API {Api}.",
            isbns.Count,
            cacheResult.Found.Count,
            dbResult.Found.Count,
            apiResult.Found.Count
        );

        return allBooks;
    }

    public async Task<List<Book>> FindBooksByTitleAsync(List<string> titles, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(titles);
        if (titles.Count == 0)
            return [];
        
        ct.ThrowIfCancellationRequested();
        
        var distinctTitles = titles
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        
        var cacheBooks = await _bookCacheService.GetBooksByTitlesAsync(distinctTitles, ct);

        var foundTitles = cacheBooks
            .Select(b => b.Title)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingTitles = distinctTitles
            .Where(t => !foundTitles.Contains(t))
            .ToList();
        
        List<Book> databaseBooks = [];
        if (missingTitles.Count > 0)
        {
            databaseBooks = await _unitOfWork.Books.GetDetailedBooksByTitleAsync(missingTitles);

            var foundTitlesInDb = databaseBooks
                .Select(b => b.Title)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            missingTitles = missingTitles
                .Where(t => !foundTitlesInDb.Contains(t))
                .ToList();
        }

        List<Book> apiBooks = [];
        if (missingTitles.Count > 0)
        {
            apiBooks = await _bookEnrichmentService.AggregateBooksByTitlesAsync(missingTitles, ct);
        }

        var booksToCache = databaseBooks.Concat(apiBooks).ToList();
        if (booksToCache.Count != 0)
        {
            await _bookCacheService.CacheBooksByTitlesAsync(booksToCache, ct);
        }
        
        _logger.LogInformation(
            "Book lookup by Titles completed. Requested {RequestedCount}. Cache: {CacheCount}, DB: {DbCount}, API: {ApiCount}. Final: {Total}.",
            titles.Count,
            cacheBooks.Count,
            databaseBooks.Count,
            apiBooks.Count,
            cacheBooks.Count + databaseBooks.Count + apiBooks.Count
        );

        return cacheBooks
            .Concat(databaseBooks)
            .Concat(apiBooks)
            .GroupBy(b => b.Isbn13 ?? b.Isbn10)
            .Select(g => g.First())
            .ToList();
    }
    
    public async Task<List<Book>> FindBooksByAuthorAsync(string author, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(author);
        if (string.IsNullOrWhiteSpace(author))
            return [];
        
        ct.ThrowIfCancellationRequested();
        
        var databaseBooks = await _unitOfWork.Books.GetBooksByAuthorAsync(author);
        if (databaseBooks.Count > 0)
            return databaseBooks;
        
        var apiBooks = await _isbndbAuthorService.GetBooksByAuthorAsync(author, ct);
        
        _logger.LogInformation(
            "Book lookup by Author completed. Requested '{Author}'. DB: {DbCount}, API: {ApiCount}. Final: {Total}.",
            author,
            databaseBooks.Count,
            apiBooks.Count,
            databaseBooks.Count + apiBooks.Count
        );
        
        return apiBooks;
    }

    public async Task<List<Book>> FindBooksByPublisherAsync(string publisher, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(publisher);
        if (string.IsNullOrWhiteSpace(publisher))
            return [];
        
        ct.ThrowIfCancellationRequested();
        
        var databaseBooks = await _unitOfWork.Books.GetBooksByPublisherAsync(publisher);
        if (databaseBooks.Count > 0)
            return databaseBooks.ArrangeBooks().ToList();
        
        var apiBooks = await _isbndbPublisherService.GetBooksByPublisherAsync(publisher, ct);
        
        _logger.LogInformation(
            "Book lookup by publisher completed. Requested '{PublisherName}'. DB: {DbCount}, API: {ApiCount}. Final: {Total}.",
            publisher,
            databaseBooks.Count,
            apiBooks.Count,
            databaseBooks.Count + apiBooks.Count
        );
        
        return apiBooks;
    }

    public async Task<List<Book>> GetAllBooksAsync(CancellationToken ct)
    {
        var databaseBooks = await _unitOfWork.Books.GetAllBooksAsync();
        if (databaseBooks.Count == 0)
            return [];
        
        await _bookCacheService.SetBooksByIsbnsAsync(databaseBooks, ct);
        
        _logger.LogInformation("Book lookup by all books completed. DB: {DbCount}.", databaseBooks.Count);
        
        return databaseBooks;
    }

    public async Task<Book?> FindBookById(int id, CancellationToken ct)
    {
        var databaseBook = await _unitOfWork.Books.GetBookByIdAsync(id);
        
        _logger.LogInformation("Book lookup by Id completed. Id: {BookId}.", id);
        
        return databaseBook ?? null;
    }
    
    private async Task<LookupResult> GetFromCacheAsync(List<Isbn> isbns, CancellationToken ct)
    {
        var books = await _bookCacheService.GetBooksByIsbnsAsync(isbns, ct);

        var foundKeys = books
            .Select(BookExtensions.GetIsbnKey)
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .ToHashSet();

        var missing = isbns
            .Where(i => !foundKeys.Contains(i.Value))
            .ToList();

        return new LookupResult(books, missing);
    }
    
    private async Task<LookupResult> GetFromDatabaseAsync(List<Isbn> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return new LookupResult([], []);

        var books = await _unitOfWork.Books
            .GetDetailedBooksByIsbnsAsync(isbns);

        var foundKeys = books
            .Select(BookExtensions.GetIsbnKey)
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .ToHashSet();

        var missing = isbns
            .Where(i => !foundKeys.Contains(i.Value))
            .ToList();

        return new LookupResult(books, missing);
    }

    private async Task<LookupResult> GetFromApiAsync(List<Isbn> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return new LookupResult([], []);

        var books = await _bookEnrichmentService
            .AggregateBooksByIsbnsAsync(isbns, ct);

        return new LookupResult(books, []);
    }
    
    private sealed record LookupResult(IReadOnlyList<Book> Found, List<Isbn> Missing);
}