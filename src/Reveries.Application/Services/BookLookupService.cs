using Reveries.Application.Common.Validation;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces.Persistence;

namespace Reveries.Application.Services;

public class BookLookupService : IBookLookupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookEnrichmentService _bookEnrichmentService;
    private readonly IIsbndbAuthorService _isbndbAuthorService;
    private readonly IIsbndbPublisherService _isbndbPublisherService;

    public BookLookupService(IUnitOfWork unitOfWork, IBookEnrichmentService bookEnrichmentService, IIsbndbAuthorService isbndbAuthorService, IIsbndbPublisherService isbndbPublisherService)
    {
        _unitOfWork = unitOfWork;
        _bookEnrichmentService = bookEnrichmentService;
        _isbndbAuthorService = isbndbAuthorService;
        _isbndbPublisherService = isbndbPublisherService;
    }
    
    public async Task<List<Book>> FindBooksByIsbnAsync(List<string> isbns, CancellationToken cancellationToken = default)
    {
        var validatedIsbns = IsbnValidationHelper.ValidateIsbns(isbns);
        if (validatedIsbns.Count == 0)
            return new List<Book>();
        
        var databaseBooks = await _unitOfWork.Books.GetBooksWithDetailsByIsbnAsync(validatedIsbns);
        
        var foundIsbns = databaseBooks
            .Select(b => b.Isbn13 ?? b.Isbn10)
            .Where(i => !string.IsNullOrWhiteSpace(i))
            .ToHashSet();
        
        var missingIsbns = isbns
            .Where(i => !foundIsbns.Contains(i))
            .ToList();
        
        if (missingIsbns.Count == 0)
            return databaseBooks;
        
        var apiBooks = await _bookEnrichmentService.MergeBooksFromSourcesByIsbnsAsync(missingIsbns, cancellationToken);
        
        return databaseBooks.Concat(apiBooks).ToList();
    }

    public async Task<List<Book>> FindBooksByTitleAsync(List<string> titles, CancellationToken cancellationToken = default)
    {
        if (titles.Count == 0)
            return new List<Book>();
        
        var booksInDatabase = await _unitOfWork.Books.GetBooksWithDetailsByTitlesAsync(titles);
        
        var dbTitles = booksInDatabase
            .Select(b => b.Title)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        var missingTitles = titles
            .Where(t => !dbTitles.Any(dbTitle =>
                dbTitle.Contains(t, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        
        if (missingTitles.Count == 0)
            return booksInDatabase;
        
        var booksFromApis = await _bookEnrichmentService.SearchBooksByTitleAsync(missingTitles, cancellationToken);
        
        return booksInDatabase.Concat(booksFromApis).ToList();
    }

    public async Task<List<Book>> FindBooksByAuthorAsync(string author, CancellationToken cancellationToken = default)
    {
        var booksInDatabase = await _unitOfWork.Books.GetBooksByAuthorAsync(author);
        if (booksInDatabase.Count > 0)
            return booksInDatabase;
        
        var booksFromApi = await _isbndbAuthorService.GetBooksForAuthorAsync(author, cancellationToken);
        return booksFromApi;
    }

    public async Task<List<Book>> FindBooksByPublisherAsync(string? publisher, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(publisher))
            return new List<Book>();
        
        var booksInDatabase = await _unitOfWork.Books.GetBooksByPublisherAsync(publisher);
        if (booksInDatabase.Count > 0)
            return booksInDatabase;
        
        var booksFromApi = await _isbndbPublisherService.GetBooksByPublisherAsync(publisher, cancellationToken);
        return booksFromApi;
    }
}