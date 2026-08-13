using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Exceptions;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Domain.Models;
using Reveries.Domain.ValueObjects;

namespace Reveries.Application.Books.Queries.FindBooksByTitles;

public sealed class FindBooksByTitlesHandler : IQueryHandler<FindBooksByTitlesQuery, List<Book>>
{
    private readonly IBookLookupService _lookupService;
    private readonly IBookRepository _books;
    private readonly IBookCacheService _cacheService;
    private readonly ILogger<FindBooksByTitlesHandler> _logger;

    public FindBooksByTitlesHandler(
        IBookLookupService lookupService,
        IBookRepository books,
        IBookCacheService cacheService,
        ILogger<FindBooksByTitlesHandler> logger)
    {
        _lookupService = lookupService;
        _books = books;
        _cacheService = cacheService;
        _logger = logger;
    }
    
    public async ValueTask<List<Book>> Handle(FindBooksByTitlesQuery query, CancellationToken ct)
    {
        var titles = query.Titles;
        
        // Cache
        var cacheResult = await GetFromCacheAsync(titles, ct);
        
        // Database
        var dbResult = await GetFromDatabaseAsync(titles, ct);

        // External API
        var apiResult = await _lookupService.LookupByTitlesAsync(dbResult.NotFound, ct);

        if (apiResult.NoResults && dbResult.NoResults)
            throw new NotFoundException($"Books with titles '{titles}' were not found.");
        
        var booksToCache = dbResult.Found.Concat(apiResult.Found).ToList();
        if (booksToCache.Count != 0)
        {
            await _cacheService.CacheBooksByTitlesAsync(booksToCache, ct);
        }
        
        var allBooks = cacheResult.Found
            .Concat(dbResult.Found)
            .Concat(apiResult.Found)
            .ToList();
        
        _logger.LogInformation(
            "Book lookup by Titles completed. Requested {RequestedCount}. Cache: {CacheCount}, DB: {DbCount}, API: {ApiCount}",
            titles.Count,
            cacheResult.Found.Count,
            dbResult.Found.Count,
            apiResult.Found.Count
        );

        return allBooks;
    }
    
    private async Task<BookLookupResult<Title>> GetFromCacheAsync(List<Title> titles, CancellationToken ct)
    {
        var books = await _cacheService.GetBooksByTitlesAsync(titles, ct);

        var foundKeys = books
            .Select(b => b.Title)
            .ToHashSet();

        var missingTitles = titles
            .Where(t => !foundKeys.Contains(t))
            .ToList();

        return new BookLookupResult<Title>(books, missingTitles);
    }
    
    private async Task<BookLookupResult<Title>> GetFromDatabaseAsync(List<Title> titles, CancellationToken ct)
    {
        if (titles.Count == 0)
            return BookLookupResult<Title>.Empty;

        var books = await _books
            .GetDetailedBooksByTitleAsync(titles, ct);

        var foundKeys = books
            .Select(b => b.Title)
            .ToHashSet();

        var missingTitles = titles
            .Where(t => !foundKeys.Contains(t))
            .ToList();

        return new BookLookupResult<Title>(books, missingTitles);
    }
}