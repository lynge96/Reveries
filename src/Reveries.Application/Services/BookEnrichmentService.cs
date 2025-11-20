using Reveries.Application.Interfaces.GoogleBooks;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Enums;
using Reveries.Core.Models;
using Reveries.Core.Services;

namespace Reveries.Application.Services;

public class BookEnrichmentService : IBookEnrichmentService
{
    private readonly IIsbndbBookService _isbndbService;
    private readonly IGoogleBookService _googleService;

    public BookEnrichmentService(IIsbndbBookService isbndbBookService, IGoogleBookService googleBookService)
    {
        _isbndbService = isbndbBookService;
        _googleService = googleBookService;
    }
    
    public async Task<List<Book>> AggregateBooksByIsbnsAsync(List<string> isbns, CancellationToken cancellationToken = default)
    {
        // Lav try-catch på NotFoundException på hver datakilde
        var googleBooksTask = _googleService.GetBooksByIsbnsAsync(isbns, cancellationToken);
        var isbndbTask = _isbndbService.GetBooksByIsbnsAsync(isbns, cancellationToken);
        
        await Task.WhenAll(googleBooksTask, isbndbTask);
        
        var googleDict = BuildIsbnDictionary(googleBooksTask.Result);
        var isbndbDict = BuildIsbnDictionary(isbndbTask.Result);
        
        var mergedBooks = new List<Book>();
        foreach (var isbn in isbns)
        {
            isbndbDict.TryGetValue(isbn, out var isbndbBook);
            googleDict.TryGetValue(isbn, out var googleBook);

            mergedBooks.Add(BookMerger.MergeBooks(isbndbBook, googleBook));
        }

        return mergedBooks;
    }

    public async Task<List<Book>> AggregateBooksByTitlesAsync(List<string> titles, CancellationToken cancellationToken = default)
    {
        var googleBooksTask = _googleService.GetBooksByTitleAsync(titles, cancellationToken);
        var isbndbTask = _isbndbService.GetBooksByTitlesAsync(titles, null, cancellationToken);
        await Task.WhenAll(googleBooksTask, isbndbTask);
        
        var googleBooks = googleBooksTask.Result;
        var isbndbBooks = isbndbTask.Result;
        
        var isbndbByIsbn = isbndbBooks
            .Where(b => !string.IsNullOrWhiteSpace(b.Isbn13))
            .ToDictionary(b => b.Isbn13!, b => b);

        var mergedBooks = new List<Book>();

        foreach (var googleBook in googleBooks)
        {
            if (!string.IsNullOrWhiteSpace(googleBook.Isbn13) && isbndbByIsbn.TryGetValue(googleBook.Isbn13, out var isbndbBook))
            {
                var mergedBook = BookMerger.MergeBooks(isbndbBook, googleBook);
                mergedBooks.Add(mergedBook);

                isbndbByIsbn.Remove(googleBook.Isbn13);
            }
            else
            {
                mergedBooks.Add(googleBook);
            }
        }

        mergedBooks.AddRange(isbndbByIsbn.Values);

        return mergedBooks
            .Where(b => !string.IsNullOrWhiteSpace(b.Isbn10))
            .Where(b => !string.IsNullOrWhiteSpace(b.Isbn13))
            .ToList();
    }
    
    private static Dictionary<string, Book> BuildIsbnDictionary(IEnumerable<Book> books)
    {
        return books
            .SelectMany(b => new[]
            {
                (isbn: b.Isbn10, book: b),
                (isbn: b.Isbn13, book: b)
            })
            .ToDictionary(x => x.isbn!, x => x.book);
    }
}