using Reveries.Application.Common.Validation;
using Reveries.Application.Extensions;
using Reveries.Application.Interfaces.Cache;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Persistence;
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

        var foundInCacheIsbns = cacheBooks
            .Select(b => b.Isbn13 ?? b.Isbn10)
            .Where(i => !string.IsNullOrWhiteSpace(i))
            .ToHashSet();

        var missingFromCache = validatedIsbns.Where(i => !foundInCacheIsbns.Contains(i)).ToList();

        var databaseBooks = missingFromCache.Count != 0
            ? await _unitOfWork.Books.GetDetailedBooksByIsbnsAsync(missingFromCache)
            : new List<Book>();

        var foundInDbIsbns = databaseBooks
            .Select(b => b.Isbn13 ?? b.Isbn10)
            .Where(i => !string.IsNullOrWhiteSpace(i))
            .ToHashSet();

        var missingIsbns = missingFromCache.Where(i => !foundInDbIsbns.Contains(i)).ToList();

        var apiBooks = missingIsbns.Count != 0
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

        var cacheBooks = await _bookCacheService.GetBooksByTitlesAsync(titles, cancellationToken);

        var foundTitles = cacheBooks
            .Select(b => b.Title)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingTitles = titles
            .Where(t => !foundTitles.Contains(t))
            .ToList();
        
        var databaseBooks = missingTitles.Count > 0
            ? await _unitOfWork.Books.GetDetailedBooksByTitleAsync(missingTitles)
            : new List<Book>();

        var foundTitlesInDb = databaseBooks
            .Select(b => b.Title)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        missingTitles = missingTitles
            .Where(t => !foundTitlesInDb.Contains(t))
            .ToList();

        var apiBooks = missingTitles.Count > 0
            ? await _bookEnrichmentService.AggregateBooksByTitlesAsync(missingTitles, cancellationToken)
            : new List<Book>();

        var booksToCache = databaseBooks.Concat(apiBooks).ToList();
        if (booksToCache.Count != 0)
        {
            var titleIsbnMap = booksToCache
                .GroupBy(b => b.Title, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(b => b.Isbn13 ?? b.Isbn10)
                        .Where(i => !string.IsNullOrWhiteSpace(i))
                        .ToList()
                );

            var allIsbns = titleIsbnMap.Values.SelectMany(x => x).Distinct().ToList();
            
            await _bookCacheService.SetIsbnsByTitleAsync(titleIsbnMap.Keys, allIsbns!, cancellationToken);
            
            await _bookCacheService.SetBooksByIsbnsAsync(booksToCache, cancellationToken);
        }
        
        return cacheBooks
            .Concat(databaseBooks)
            .Concat(apiBooks)
            .GroupBy(b => b.Isbn13 ?? b.Isbn10)
            .Select(g => g.First())
            .ToList();
    }
    
    public async Task<List<Book>> FindBooksByAuthorAsync(string author, CancellationToken cancellationToken = default)
    {
        var databaseBooks = await _unitOfWork.Books.GetBooksByAuthorAsync(author);
        if (databaseBooks.Count > 0)
            return databaseBooks;
        
        var apiBooks = await _isbndbAuthorService.GetBooksByAuthorAsync(author, cancellationToken);
        return apiBooks;
    }

    public async Task<List<Book>> FindBooksByPublisherAsync(string? publisher, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(publisher))
            return new List<Book>();
        
        var databaseBooks = await _unitOfWork.Books.GetBooksByPublisherAsync(publisher);
        if (databaseBooks.Count > 0)
            return databaseBooks.ArrangeBooks().ToList();
        
        var apiBooks = await _isbndbPublisherService.GetBooksByPublisherAsync(publisher, cancellationToken);
        return apiBooks;
    }

    public async Task<List<Book>> GetAllBooksAsync(CancellationToken cancellationToken = default)
    {
        var databaseBooks = await _unitOfWork.Books.GetAllBooksAsync();
        if (databaseBooks.Count == 0)
            return new List<Book>();
        
        await _bookCacheService.SetBooksByIsbnsAsync(databaseBooks, cancellationToken);
        
        return databaseBooks;
    }
}