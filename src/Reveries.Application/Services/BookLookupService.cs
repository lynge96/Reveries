using Reveries.Application.Common.Validation;
using Reveries.Application.Extensions;
using Reveries.Application.Interfaces.Cache;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Persistence;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Entities;

namespace Reveries.Application.Services;

public class BookLookupService : IBookLookupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookEnrichmentService _bookEnrichmentService;
    private readonly IIsbndbAuthorService _isbndbAuthorService;
    private readonly IIsbndbPublisherService _isbndbPublisherService;
    private readonly IBookCacheService _bookCacheService;

    public BookLookupService(IUnitOfWork unitOfWork, IBookEnrichmentService bookEnrichmentService, IIsbndbAuthorService isbndbAuthorService, IIsbndbPublisherService isbndbPublisherService, IBookCacheService bookCacheService)
    {
        _unitOfWork = unitOfWork;
        _bookEnrichmentService = bookEnrichmentService;
        _isbndbAuthorService = isbndbAuthorService;
        _isbndbPublisherService = isbndbPublisherService;
        _bookCacheService = bookCacheService;
    }
    
    public async Task<List<Book>> FindBooksByIsbnAsync(List<string> isbns, CancellationToken cancellationToken = default)
    {
        var validatedIsbns = IsbnValidationHelper.ValidateIsbns(isbns);
        if (validatedIsbns.Count == 0) return new List<Book>();
        
        var cacheBooks = await _bookCacheService.GetBooksByIsbnsAsync(validatedIsbns, cancellationToken);
        
        var foundInCache = cacheBooks
            .Select(b => b.Isbn13 ?? b.Isbn10)
            .Where(i => !string.IsNullOrWhiteSpace(i))
            .ToHashSet();
        
        var missingIsbns = validatedIsbns.Where(i => !foundInCache.Contains(i)).ToList();
        
        var databaseBooks = await _unitOfWork.Books.GetDetailedBooksByIsbnsAsync(missingIsbns);
        
        var foundInDb  = databaseBooks
            .Select(b => b.Isbn13 ?? b.Isbn10)
            .Where(i => !string.IsNullOrWhiteSpace(i))
            .ToHashSet();
        
        missingIsbns = missingIsbns.Where(i => !foundInDb.Contains(i)).ToList();
        
        var apiBooks = missingIsbns.Count > 0
            ? await _bookEnrichmentService.AggregateBooksByIsbnsAsync(missingIsbns, cancellationToken)
            : new List<Book>();

        await _bookCacheService.SetBooksByIsbnsAsync(databaseBooks.Concat(apiBooks), cancellationToken);
        
        return cacheBooks
            .Concat(databaseBooks)
            .Concat(apiBooks)
            .ToList();
    }

    public async Task<List<Book>> FindBooksByTitleAsync(List<string> titles, CancellationToken cancellationToken = default)
    {
        if (titles.Count == 0)
            return new List<Book>();
        // TODO: tilføj cache
        var booksInDatabase = await _unitOfWork.Books.GetDetailedBooksByTitleAsync(titles);
        
        var dbTitles = booksInDatabase
            .Select(b => b.Title)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        var missingTitles = titles
            .Where(t => !dbTitles.Any(dbTitle =>
                dbTitle.Contains(t, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        
        if (missingTitles.Count == 0)
            return booksInDatabase;
        
        var booksFromApis = await _bookEnrichmentService.AggregateBooksByTitlesAsync(missingTitles, cancellationToken);
        
        return booksInDatabase.Concat(booksFromApis).ToList();
    }

    public async Task<List<Book>> FindBooksByAuthorAsync(string author, CancellationToken cancellationToken = default)
    {
        var booksInDatabase = await _unitOfWork.Books.GetBooksByAuthorAsync(author);
        if (booksInDatabase.Count > 0)
            return booksInDatabase;
        
        var booksFromApi = await _isbndbAuthorService.GetBooksByAuthorAsync(author, cancellationToken);
        return booksFromApi;
    }

    public async Task<List<Book>> FindBooksByPublisherAsync(string? publisher, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(publisher))
            return new List<Book>();
        
        var booksInDatabase = await _unitOfWork.Books.GetBooksByPublisherAsync(publisher);
        if (booksInDatabase.Count > 0)
            return booksInDatabase.ArrangeBooks().ToList();
        
        var booksFromApi = await _isbndbPublisherService.GetBooksByPublisherAsync(publisher, cancellationToken);
        return booksFromApi;
    }

    public async Task<List<Book>> GetAllBooksAsync(CancellationToken cancellationToken = default)
    {
        var books = await _unitOfWork.Books.GetAllBooksAsync();
        if (books.Count == 0)
            return new List<Book>();
        
        return books;
    }
}