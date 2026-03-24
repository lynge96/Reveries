using Microsoft.Extensions.Logging;
using Reveries.Application.Interfaces.Books;
using Reveries.Core.Helpers;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using BookExtensions = Reveries.Application.Extensions.BookExtensions;

namespace Reveries.Application.Services;

public class BookEnrichmentService
{
    private readonly IIsbndbBookSearch _isbndbSearch;
    private readonly IGoogleBookSearch _googleSearch;
    private readonly ILogger<BookEnrichmentService> _logger;

    public BookEnrichmentService(
        IIsbndbBookSearch isbndbBookSearch,
        IGoogleBookSearch googleBookSearch, 
        ILogger<BookEnrichmentService> logger)
    {
        _isbndbSearch = isbndbBookSearch;
        _googleSearch = googleBookSearch;
        _logger = logger;
    }
    
    public async Task<List<Book>?> AggregateBooksByIsbnsAsync(List<Isbn> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return [];

        var results = await Task.WhenAll(
            _isbndbSearch.GetBooksByIsbnsAsync(isbns, ct),
            _googleSearch.GetBooksByIsbnsAsync(isbns, ct)
        );
        var isbndbBooks = results[0];
        if (isbndbBooks is null) 
            _logger.LogDebug("No books found in ISBNDB for {Isbns} ISBNs.", string.Join(", ", isbns.Select(i => i.Value)));
        
        var googleBooks = results[1];
        if (googleBooks is null) 
            _logger.LogDebug("No books found in Google Books for {Isbns} ISBNs.", string.Join(", ", isbns.Select(i => i.Value)));
        
        if (googleBooks is null && isbndbBooks is null)
            return null;
        
        var googleDict = BuildIsbnDictionary(googleBooks ?? []);
        var isbndbDict = BuildIsbnDictionary(isbndbBooks ?? []);

        var mergedBooks = isbns
            .Select(isbn =>
            {
                isbndbDict.TryGetValue(isbn.Value, out var isbndbBook);
                googleDict.TryGetValue(isbn.Value, out var googleBook);

                return BookMerger.MergeBooks(isbndbBook, googleBook);
            })
            .Where(b => b is not null)
            .Select(b => b!)
            .ToList();
        
        _logger.LogDebug("Aggregated {MergedCount} books from {IsbnCount} ISBNs.", mergedBooks.Count, isbns.Count);
        return mergedBooks;
    }

    public async Task<List<Book>?> AggregateBooksByTitlesAsync(List<string> titles, CancellationToken ct)
    {
        if (titles.Count == 0)
            return [];
        
        var results = await Task.WhenAll(
            _isbndbSearch.GetBooksByTitlesAsync(titles, null, ct),
            _googleSearch.GetBooksByTitlesAsync(titles, ct)
        );
        var isbndbBooks = results[0];
        if (isbndbBooks is null) 
            _logger.LogDebug("No books found in ISBNDB for {Titles}.", string.Join(", ", titles));
        
        var googleBooks = results[1];
        if (googleBooks is null) 
            _logger.LogDebug("No books found in Google Books for {Titles}.", string.Join(", ", titles));
        
        if (googleBooks is null && isbndbBooks is null)
            return null;
        
        var mergedByIsbn = MergeBookDictionaries(googleBooks ?? [], isbndbBooks ?? []);
        
        var mergedBooks = mergedByIsbn.Values
            .Where(b =>
                !string.IsNullOrWhiteSpace(b.Isbn13?.Value) ||
                !string.IsNullOrWhiteSpace(b.Isbn10?.Value))
            .ToList();

        _logger.LogDebug("Aggregated {MergedCount} books from {TitleCount} titles.", mergedBooks.Count, titles.Count);
        return mergedBooks;
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
    
    private static Dictionary<string, Book> MergeBookDictionaries(IEnumerable<Book> primary, IEnumerable<Book> secondary)
    {
        var secondaryDict = secondary
            .Select(b => new { Book = b, Key = BookExtensions.GetIsbnKey(b) })
            .Where(x => x.Key is not null)
            .GroupBy(x => x.Key!)
            .ToDictionary(g => g.Key, g => g.First().Book);

        var mergedByIsbn = new Dictionary<string, Book>();

        foreach (var primaryBook in primary)
        {
            var key = BookExtensions.GetIsbnKey(primaryBook);
            if (key is null)
                continue;

            if (secondaryDict.TryGetValue(key, out var secondaryBook))
            {
                mergedByIsbn[key] = BookMerger.MergeBooks(secondaryBook, primaryBook)!;
                secondaryDict.Remove(key);
            }
            else
            {
                mergedByIsbn[key] = primaryBook;
            }
        }

        foreach (var (key, book) in secondaryDict)
            mergedByIsbn.TryAdd(key, book);

        return mergedByIsbn;
    }
}