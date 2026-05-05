using Microsoft.Extensions.Logging;
using Reveries.Application.Authors.Interfaces;
using Reveries.Application.Books.Extensions;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Common.Abstractions;
using Reveries.Application.Common.Exceptions;
using Reveries.Application.Publishers.Interfaces;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Services;

public class BookLookupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BookEnrichmentService _bookEnrichmentService;
    private readonly IAuthorSearch _authorSearch;
    private readonly IPublisherSearch _publisherSearch;
    private readonly IBookCacheService _bookCacheService;
    private readonly ILogger<BookLookupService> _logger;

    public BookLookupService(
        IUnitOfWork unitOfWork, 
        BookEnrichmentService bookEnrichmentService, 
        IAuthorSearch authorSearch, 
        IPublisherSearch publisherSearch, 
        IBookCacheService bookCacheService, 
        ILogger<BookLookupService> logger)
    {
        _unitOfWork = unitOfWork;
        _bookEnrichmentService = bookEnrichmentService;
        _authorSearch = authorSearch;
        _publisherSearch = publisherSearch;
        _bookCacheService = bookCacheService;
        _logger = logger;
    }
    
    public async Task<Book> FindBookByIsbnAsync(Isbn isbn, CancellationToken ct)
    {
        var books = await FindBooksByIsbnAsync([isbn], ct);
        
        return books.FirstOrDefault() ?? throw new NotFoundException($"Book with ISBN '{isbn}' was not found.");
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
            "Book lookup completed. Requested {Requested}. Cache {Cache}, DB {Db}, API {Api}",
            isbns.Count,
            cacheResult.Found.Count,
            dbResult.Found.Count,
            apiResult.Found.Count
        );

        return allBooks;
    }

    public async Task<List<Book>> FindBooksByTitleAsync(List<string> titles, CancellationToken ct)
    {
        if (titles.Count == 0)
            return [];
        
        var distinctTitles = titles
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        
        var cacheResult = await GetFromCacheAsync(distinctTitles, ct);
        var dbResult = await GetFromDatabaseAsync(cacheResult.Missing, ct);
        var apiResult = await GetFromApiAsync(dbResult.Missing, ct);

        var allBooks = cacheResult.Found
            .Concat(dbResult.Found)
            .Concat(apiResult.Found)
            .ToList();
        
        var booksToCache = dbResult.Found.Concat(apiResult.Found).ToList();
        if (booksToCache.Count != 0)
        {
            await _bookCacheService.CacheBooksByTitlesAsync(booksToCache, ct);
        }
        
        _logger.LogInformation(
            "Book lookup by Titles completed. Requested {RequestedCount}. Cache: {CacheCount}, DB: {DbCount}, API: {ApiCount}",
            distinctTitles.Count,
            cacheResult.Found.Count,
            dbResult.Found.Count,
            apiResult.Found.Count
        );

        return allBooks;
    }
    
    public async Task<List<Book>> FindBooksByAuthorAsync(string author, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(author))
            return [];
        
        var databaseBooks = await _unitOfWork.Books.GetBooksByAuthorAsync(author, ct);
        if (databaseBooks.Count > 0)
            return databaseBooks;
        
        var apiBooks = await _authorSearch.GetBooksByAuthorAsync(author, ct);

        if (apiBooks is null)
            throw new NotFoundException($"Books with author '{author}' were not found.");
        
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
        if (string.IsNullOrWhiteSpace(publisher))
            return [];
        
        var databaseBooks = await _unitOfWork.Books.GetBooksByPublisherAsync(publisher, ct);
        if (databaseBooks.Count > 0)
            return databaseBooks.ArrangeBooks().ToList();
        
        var apiBooks = await _publisherSearch.GetBooksByPublisherAsync(publisher, ct);

        if (apiBooks is null)
            throw new NotFoundException($"Books with publisher '{publisher}' were not found.");
        
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
    
    private async Task<LookupResult<Isbn>> GetFromCacheAsync(List<Isbn> isbns, CancellationToken ct)
    {
        var books = await _bookCacheService.GetBooksByIsbnsAsync(isbns, ct);

        var foundKeys = books
            .Select(BookExtensions.GetIsbnKey)
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .ToHashSet();

        var missing = isbns
            .Where(i => !foundKeys.Contains(i.Value))
            .ToList();

        return new LookupResult<Isbn>(books, missing);
    }
    
    private async Task<LookupResult<string>> GetFromCacheAsync(List<string> titles, CancellationToken ct)
    {
        var books = await _bookCacheService.GetBooksByTitlesAsync(titles, ct);

        var foundKeys = books
            .Select(b => b.Title)
            .ToHashSet();

        var missing = titles
            .Where(t => !foundKeys.Contains(t))
            .ToList();

        return new LookupResult<string>(books, missing);
    }
    
    private async Task<LookupResult<Isbn>> GetFromDatabaseAsync(List<Isbn> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return new LookupResult<Isbn>([], []);

        var books = await _unitOfWork.Books
            .GetDetailedBooksByIsbnsAsync(isbns, ct);

        var foundKeys = books
            .Select(BookExtensions.GetIsbnKey)
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .ToHashSet();

        var missing = isbns
            .Where(i => !foundKeys.Contains(i.Value))
            .ToList();

        return new LookupResult<Isbn>(books, missing);
    }

    private async Task<LookupResult<string>> GetFromDatabaseAsync(List<string> titles, CancellationToken ct)
    {
        if (titles.Count == 0)
            return new LookupResult<string>([], []);

        var books = await _unitOfWork.Books
            .GetDetailedBooksByTitleAsync(titles, ct);

        var foundKeys = books
            .Select(b => b.Title)
            .ToHashSet();

        var missing = titles
            .Where(t => !foundKeys.Contains(t))
            .ToList();

        return new LookupResult<string>(books, missing);
    }
    
    private async Task<LookupResult<Isbn>> GetFromApiAsync(List<Isbn> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return new LookupResult<Isbn>([], []);

        var books = await _bookEnrichmentService
            .AggregateBooksByIsbnsAsync(isbns, ct);

        if (books is null)
            throw new NotFoundException($"Books with ISBNs '{string.Join(", ", isbns.Select(i => i.Value))}' were not found.");

        return new LookupResult<Isbn>(books, []);
    }
    
    private async Task<LookupResult<string>> GetFromApiAsync(List<string> titles, CancellationToken ct)
    {
        if (titles.Count == 0)
            return new LookupResult<string>([], []);

        var books = await _bookEnrichmentService
            .AggregateBooksByTitlesAsync(titles, ct);
        
        if (books is null)
            throw new NotFoundException($"Books with titles '{string.Join(", ", titles)}' were not found.");
        
        return new LookupResult<string>(books, []);
    }
    
    private sealed record LookupResult<T>(IReadOnlyList<Book> Found, List<T> Missing);
}