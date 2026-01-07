using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Reveries.Application.Extensions;
using Reveries.Application.Interfaces.Cache;
using Reveries.Core.Enums;
using Reveries.Core.Models;
using Reveries.Infrastructure.Redis.Configuration;

namespace Reveries.Infrastructure.Redis.Services;

public class BookCacheService : IBookCacheService
{
    private readonly ICacheService _cache;
    private readonly ILogger<BookCacheService> _logger;
    
    private readonly StringComparer _titleComparer = StringComparer.OrdinalIgnoreCase;
    
    public BookCacheService(ICacheService cacheService, ILogger<BookCacheService> logger)
    {
        _cache = cacheService;
        _logger = logger;
    }
    
    public async Task<Book?> GetBookByIsbnAsync(string isbn, CancellationToken ct)
    {
        var key = CacheKeys.BookByIsbn(isbn);
        
        return await _cache.GetAsync<Book>(key, ct);
    }

    public async Task SetBookByIsbnAsync(Book book, CancellationToken ct)
    {
        if (book.Isbn13 == null && book.Isbn10 == null) return;
        
        var key = CacheKeys.BookByIsbn(book.Isbn13 ?? book.Isbn10!);
        
        await _cache.SetAsync(key, book, CacheDefaults.DefaultExpiration, ct);
    }

    public async Task RemoveBookByIsbnAsync(string? isbn, CancellationToken ct)
    {
        if (isbn != null)
        {
            var key = CacheKeys.BookByIsbn(isbn);
        
            await _cache.RemoveAsync(key, ct);
        }
    }
    
    public async Task<IReadOnlyList<Book>> GetBooksByIsbnsAsync(IEnumerable<string> isbns, CancellationToken ct)
    {
        var distinctIsbns = isbns.Distinct().ToList();
        
        var tasks = distinctIsbns.Select(isbn => GetBookByIsbnAsync(isbn, ct));
        var books = await Task.WhenAll(tasks);
        
        var updatedBooks = books
            .Where(b => b is not null)
            .Select(b => b!.UpdateDataSource(DataSource.Cache))
            .ToImmutableList();

        _logger.LogDebug("Cache ISBN lookup completed. Requested {IsbnCount} Isbns, found {BooksCount} books in cache.", distinctIsbns.Count, updatedBooks.Count);
        return updatedBooks;
    }

    public async Task SetBooksByIsbnsAsync(IEnumerable<Book> books, CancellationToken ct)
    {
        var tasks = books.Select(book => SetBookByIsbnAsync(book, ct));
        await Task.WhenAll(tasks);
    }

    public async Task<IReadOnlyList<Book>> GetBooksByTitlesAsync(IEnumerable<string> titles, CancellationToken ct)
    {
        var distinctTitles = titles
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(_titleComparer)
            .ToList();
        
        var batch = _cache.CreateBatch();
        var titleTasks = distinctTitles.ToDictionary(
            title => title,
            title => batch.StringGetAsync(CacheKeys.BookIsbnsByTitle(title))
        );
        batch.Execute();
        await Task.WhenAll(titleTasks.Values);
        
        var isbnResults = new Dictionary<string, List<string>>();
        foreach (var (title, task) in titleTasks)
        {
            var redisVal = await task;
            if (!redisVal.IsNullOrEmpty)
            {
                var isbns = JsonSerializer.Deserialize<List<string>>(redisVal!) ?? [];
                isbnResults[title] = isbns;
            }
        }
        
        var allIsbns = isbnResults.Values.SelectMany(x => x).Distinct().ToList();
        
        if (allIsbns.Count == 0)
            return new List<Book>();
        
        var books = await GetBooksByIsbnsAsync(allIsbns, ct);
        
        _logger.LogDebug("Cache title lookup completed. Requested {IsbnCount} titles, found {BooksCount} books in cache.", distinctTitles.Count, books.Count);
        return books.ToList();
    }

    public async Task SetIsbnsByTitleAsync(Dictionary<string, List<string>> titleIsbnMap, CancellationToken ct)
    {
        if (titleIsbnMap is null)
            throw new ArgumentNullException(nameof(titleIsbnMap));

        if (titleIsbnMap.Count == 0)
            return;

        var batch = _cache.CreateBatch();
        var tasks = new List<Task>(titleIsbnMap.Count);

        foreach (var (title, isbns) in titleIsbnMap)
        {
            if (string.IsNullOrWhiteSpace(title) || isbns.Count == 0)
                continue;

            var key = CacheKeys.BookIsbnsByTitle(title);

            var serialized = JsonSerializer.Serialize(isbns);

            tasks.Add(batch.StringSetAsync(key, serialized, CacheDefaults.DefaultExpiration));
        }

        batch.Execute();

        await Task.WhenAll(tasks);
    }

    public async Task CacheBooksByTitlesAsync(IEnumerable<Book> books, CancellationToken ct)
    {
        var booksToCache = books.ToList();

        if (booksToCache.Count == 0)
            return;

        var titleIsbnMap = booksToCache
            .Where(b => !string.IsNullOrWhiteSpace(b.Title))
            .GroupBy(b => b.Title, _titleComparer)
            .ToDictionary(
                g => g.Key,
                g => g.Select(BookExtensions.GetIsbnKey)
                    .Where(i => !string.IsNullOrWhiteSpace(i))
                    .Distinct()
                    .ToList(),
                _titleComparer);

        if (titleIsbnMap.Count == 0)
            return;

        await Task.WhenAll(
            SetIsbnsByTitleAsync(titleIsbnMap!, ct),
            SetBooksByIsbnsAsync(booksToCache, ct)
        );
    }
}