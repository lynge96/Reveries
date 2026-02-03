using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.GoogleBooks;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Exceptions;
using Reveries.Core.Helpers;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using BookExtensions = Reveries.Application.Extensions.BookExtensions;

namespace Reveries.Application.Services;

public class BookEnrichmentService : IBookEnrichmentService
{
    private readonly IIsbndbBookService _isbndbService;
    private readonly IGoogleBooksService _googleService;
    private readonly ILogger<BookEnrichmentService> _logger;

    public BookEnrichmentService(IIsbndbBookService isbndbBookService, IGoogleBooksService googleBooksService, ILogger<BookEnrichmentService> logger)
    {
        _isbndbService = isbndbBookService;
        _googleService = googleBooksService;
        _logger = logger;
    }
    
    public async Task<List<Book>> AggregateBooksByIsbnsAsync(List<Isbn> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return [];
        
        ct.ThrowIfCancellationRequested();
        
        var googleTask = FetchGoogleBooksSafeAsync(isbns, ct);
        var isbndbTask = FetchIsbndbBooksSafeAsync(isbns, ct);
        
        await Task.WhenAll(googleTask, isbndbTask);
        
        var googleBooks = await googleTask;
        var isbndbBooks = await isbndbTask;
        
        var googleDict = BuildIsbnDictionary(googleBooks);
        var isbndbDict = BuildIsbnDictionary(isbndbBooks);
        
        var mergedBooks = new List<Book>();
        
        foreach (var isbn in isbns)
        {
            isbndbDict.TryGetValue(isbn.Value, out var isbndbBook);
            googleDict.TryGetValue(isbn.Value, out var googleBook);

            mergedBooks.Add(BookMerger.MergeBooks(isbndbBook, googleBook));
        }

        return mergedBooks;
    }

    public async Task<List<Book>> AggregateBooksByTitlesAsync(List<string> titles, CancellationToken ct)
    {
        if (titles.Count == 0)
            return [];
        
        ct.ThrowIfCancellationRequested();
        
        var googleTask = _googleService.GetBooksByTitleAsync(titles, ct);
        var isbndbTask = _isbndbService.GetBooksByTitlesAsync(titles, null, ct);

        await Task.WhenAll(googleTask, isbndbTask);
        
        var googleBooks = await googleTask;
        var isbndbBooks = await isbndbTask;
        
        var isbndbByIsbn = isbndbBooks
            .Select(b => new { Book = b, Key = BookExtensions.GetIsbnKey(b) })
            .Where(x => x.Key is not null)
            .GroupBy(x => x.Key!)
            .ToDictionary(g => g.Key, g => g.First().Book);

        var mergedByIsbn = new Dictionary<string, Book>();

        foreach (var googleBook in googleBooks)
        {
            var key = BookExtensions.GetIsbnKey(googleBook);
            if (key is null)
                continue;

            if (isbndbByIsbn.TryGetValue(key, out var isbndbBook))
            {
                mergedByIsbn[key] = BookMerger.MergeBooks(isbndbBook, googleBook);
                isbndbByIsbn.Remove(key);
            }
            else
            {
                mergedByIsbn[key] = googleBook;
            }
        }

        foreach (var (key, book) in isbndbByIsbn)
        {
            mergedByIsbn.TryAdd(key, book);
        }

        var mergedBooks = mergedByIsbn.Values
            .Where(b =>
                !string.IsNullOrWhiteSpace(b.Isbn13?.Value) ||
                !string.IsNullOrWhiteSpace(b.Isbn10?.Value))
            .ToList();

        return mergedBooks;
    }
    
    private async Task<List<Book>> FetchGoogleBooksSafeAsync(List<Isbn> isbns, CancellationToken ct)
    {
        try
        {
            return await _googleService.GetBooksByIsbnsAsync(isbns, ct);
        }
        catch (NotFoundException ex)
        {
            _logger.LogInformation(ex, "No books found in Google Books for the provided ISBNs: {Isbns}", string.Join(", ", isbns));

            return [];
        }
    }
    
    private async Task<List<Book>> FetchIsbndbBooksSafeAsync(List<Isbn> isbns, CancellationToken ct)
    {
        try
        {
            return await _isbndbService.GetBooksByIsbnsAsync(isbns, ct);
        }
        catch (NotFoundException ex)
        {
            _logger.LogInformation(ex, "No books found in ISBNdb for the provided ISBNs: {Isbns}", string.Join(", ", isbns));

            return [];
        }
    }
    
    private static Dictionary<string, Book> BuildIsbnDictionary(IEnumerable<Book> books)
    {
        return books
            .SelectMany(b => new[]
            {
                (isbn: b.Isbn10?.Value, book: b),
                (isbn: b.Isbn13?.Value, book: b)
            })
            .ToDictionary(x => x.isbn!, x => x.book);
    }
    
}