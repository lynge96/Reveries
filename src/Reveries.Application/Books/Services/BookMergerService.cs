using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Core.Helpers;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using BookExtensions = Reveries.Application.Books.Extensions.BookExtensions;

namespace Reveries.Application.Books.Services;

public class BookMergerService : IBookMergerService
{
    private readonly ILogger<BookMergerService> _logger;

    public BookMergerService(ILogger<BookMergerService> logger)
    {
        _logger = logger;
    }
    
    public List<Book> AggregateBooksByIsbnsAsync(
        IReadOnlyList<Isbn> isbns,
        IReadOnlyList<Book>? isbndbBooks,
        IReadOnlyList<Book>? googleBooks)
    {
        if (isbns.Count == 0 || isbndbBooks is null && googleBooks is null)
            return [];
        
        var googleDict = BuildIsbnDictionary(googleBooks ?? []);
        var isbndbDict = BuildIsbnDictionary(isbndbBooks ?? []);

        var mergedBooks = isbns
            .Select(isbn =>
            {
                isbndbDict.TryGetValue(isbn.Value, out var isbndbBook);
                googleDict.TryGetValue(isbn.Value, out var googleBook);

                return BookMerger.MergeBooks(isbndbBook, googleBook);
            })
            .OfType<Book>()
            .ToList();
        
        _logger.LogDebug("Aggregated {MergedCount} books from {IsbnCount} ISBNs.", mergedBooks.Count, isbns.Count);
        return mergedBooks;
    }
    
    public List<Book> AggregateBooksByTitlesAsync(
        IReadOnlyList<string> titles, 
        IReadOnlyList<Book>? isbndbBooks,
        IReadOnlyList<Book>? googleBooks)
    {
        if (titles.Count == 0)
            return [];
        
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
            .Where(x => x.isbn is not null)
            .GroupBy(x => x.isbn!)
            .ToDictionary(g => g.Key, g => g.First().book);
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