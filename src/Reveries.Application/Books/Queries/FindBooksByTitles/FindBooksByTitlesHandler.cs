using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Abstractions;
using Reveries.Application.Common.Exceptions;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Queries.FindBooksByTitles;

public sealed class FindBooksByTitlesHandler : IQueryHandler<FindBooksByTitlesQuery, List<Book>>
{
    private readonly IBookLookupService _lookupService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookCacheService _cacheService;
    private readonly ILogger<FindBooksByTitlesHandler> _logger;
    
    public FindBooksByTitlesHandler(
        IBookLookupService lookupService,
        IUnitOfWork unitOfWork,
        IBookCacheService cacheService,
        ILogger<FindBooksByTitlesHandler> logger)
    {
        _lookupService = lookupService;
        _unitOfWork = unitOfWork;
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

        var books = await _unitOfWork.Books
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