using Reveries.Application.Common.Validation;
using Reveries.Application.Interfaces.GoogleBooks;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Entities;
using Reveries.Core.Enums;

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
    // TODO: Færdiggør metoderne, så de henter bøger fra ISBNDB og Google Books og samler dem sammen, så searchHandler kun bruger 1 service.
    
    public async Task<List<Book>> EnrichBooksByIsbnsAsync(List<string> isbns, CancellationToken cancellationToken = default)
    {
        var validatedIsbns = IsbnValidationHelper.ValidateIsbns(isbns);
        if (validatedIsbns.Count == 0)
            return new List<Book>();
        
        var googleBooksTask = _googleService.GetBooksByIsbnsAsync(validatedIsbns, cancellationToken);
        var isbndbTask = _isbndbService.GetBooksByIsbnsAsync(validatedIsbns, cancellationToken);
        await Task.WhenAll(googleBooksTask, isbndbTask);
        
        var googleBooks = googleBooksTask.Result;
        var isbndbBooks = isbndbTask.Result;
        
        var books = new List<Book>();
        foreach (var isbn in validatedIsbns)
        {
            var isbndbBook = isbndbBooks.FirstOrDefault(b => b.Isbn13 == isbn || b.Isbn10 == isbn);
            var googleBook = googleBooks.FirstOrDefault(b => b.Isbn13 == isbn || b.Isbn10 == isbn);

            var mergedBook = MergeBooks(isbndbBook, googleBook);
            
            books.Add(mergedBook);
        }

        return books;
    }

    public async Task<List<Book>> SearchBooksByTitleAsync(List<string> titles, CancellationToken cancellationToken = default)
    {
        var googleBooksTask = _googleService.GetBooksByTitleAsync(titles, cancellationToken);
        var isbndbTask = _isbndbService.GetBooksByTitlesAsync(titles, null, BookFormat.PhysicalOnly, cancellationToken);
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
                var mergedBook = MergeBooks(isbndbBook, googleBook);
                mergedBooks.Add(mergedBook);

                isbndbByIsbn.Remove(googleBook.Isbn13);
            }
            else
            {
                mergedBooks.Add(googleBook);
            }
        }

        mergedBooks.AddRange(isbndbByIsbn.Values);

        return mergedBooks;
    }
    
    private static Book MergeBooks(Book? isbndbBook, Book? googleBook)
    {
        if (isbndbBook == null && googleBook == null)
            return null!;
        if (isbndbBook == null)
            return googleBook!;
        if (googleBook == null)
            return isbndbBook;

        return new Book
        {
            DataSource = DataSource.CombinedBookApi,
            Isbn13 = Prefer(isbndbBook.Isbn13, googleBook.Isbn13),
            Isbn10 = Prefer(isbndbBook.Isbn10, googleBook.Isbn10),
            Title = Prefer(googleBook.Title, isbndbBook.Title)!,
            Pages = isbndbBook.Pages > 0 ? isbndbBook.Pages : googleBook.Pages,
            LanguageIso639 = isbndbBook.LanguageIso639 ?? googleBook.LanguageIso639,
            Language = isbndbBook.Language ?? googleBook.Language,
            PublishDate = isbndbBook.PublishDate ?? googleBook.PublishDate,
            Synopsis = googleBook.Synopsis ?? isbndbBook.Synopsis,
            ImageThumbnail = googleBook.ImageThumbnail ?? isbndbBook.ImageThumbnail,
            ImageUrl = isbndbBook.ImageThumbnail ?? isbndbBook.ImageUrl,
            Msrp = isbndbBook.Msrp,
            Binding = isbndbBook.Binding ?? googleBook.Binding,
            Edition = Prefer(googleBook.Edition, isbndbBook.Edition),
            SeriesNumber = isbndbBook.SeriesNumber,
            Dimensions = isbndbBook.Dimensions,
            
            // Navigation properties
            Authors = googleBook.Authors.Count != 0 ? googleBook.Authors : isbndbBook.Authors,
            Publisher = isbndbBook.Publisher ?? googleBook.Publisher,
            DeweyDecimals = isbndbBook.DeweyDecimals,
            Subjects = googleBook.Subjects != null && googleBook.Subjects.Count != 0 ? googleBook.Subjects : isbndbBook.Subjects,
            Series = isbndbBook.Series
        };
    }
    
    private static string? Prefer(params string?[] values)
    {
        return values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
    }
    
}