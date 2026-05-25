using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Extensions;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Abstractions;
using Reveries.Application.Common.Exceptions;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Queries.FindBooksByIsbns;

public sealed class FindBooksByIsbnsHandler : IQueryHandler<FindBooksByIsbnsQuery, List<Book>>
{
    private readonly IBookLookupService _lookupService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookCacheService _cacheService;
    private readonly ILogger<FindBooksByIsbnsHandler> _logger;
    
    public FindBooksByIsbnsHandler(
        IBookLookupService lookupService,
        IUnitOfWork unitOfWork,
        IBookCacheService cacheService,
        ILogger<FindBooksByIsbnsHandler> logger)
    {
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _lookupService = lookupService;
        _logger = logger;
    }
    
    public async ValueTask<List<Book>> Handle(FindBooksByIsbnsQuery query, CancellationToken ct)
    {
        var isbns = query.Isbns;
        
        // Cache
        var cacheResult = await GetFromCacheAsync(isbns, ct);
        
        // Database
        var dbResult = await GetFromDatabaseAsync(cacheResult.NotFound, ct);
        
        // External API
        var apiResult = await _lookupService.LookupByIsbnsAsync(dbResult.NotFound, ct);
        
        if (apiResult.NoResults && dbResult.NoResults)
            throw new NotFoundException($"Books with ISBNs '{isbns}' were not found.");
        
        var booksToCache = dbResult.Found.Concat(apiResult.Found).ToList();
        if (booksToCache.Count != 0)
        {
            await _cacheService.SetBooksByIsbnsAsync(booksToCache, ct);
        }
        
        var allBooks = cacheResult.Found
            .Concat(dbResult.Found)
            .Concat(apiResult.Found)
            .ToList();
        
        _logger.LogInformation(
            "Book lookup: Requested {Requested}, Cache {Cache}, DB {Db}, API {Api}",
            isbns.Count,
            cacheResult.Found.Count,
            dbResult.Found.Count,
            apiResult.Found.Count);
        
        return allBooks;
    }
    
    private async Task<BookLookupResult<Isbn>> GetFromCacheAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct)
    {
        var books = await _cacheService.GetBooksByIsbnsAsync(isbns, ct);

        var foundKeys = books
            .Select(BookExtensions.GetIsbnKey)
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .ToHashSet();

        var missingIsbns = isbns
            .Where(i => !foundKeys.Contains(i.Value))
            .ToList();

        return new BookLookupResult<Isbn>(books, missingIsbns);
    }
    
    private async Task<BookLookupResult<Isbn>> GetFromDatabaseAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return BookLookupResult<Isbn>.Empty;

        var books = await _unitOfWork.Books
            .GetDetailedBooksByIsbnsAsync(isbns, ct);

        var foundKeys = books
            .Select(BookExtensions.GetIsbnKey)
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .ToHashSet();

        var missingIsbns = isbns
            .Where(i => !foundKeys.Contains(i.Value))
            .ToList();

        return new BookLookupResult<Isbn>(books, missingIsbns);
    }
}