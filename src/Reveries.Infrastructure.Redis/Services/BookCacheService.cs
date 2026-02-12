using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Reveries.Application.Extensions;
using Reveries.Application.Interfaces.Cache;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Redis.Configuration;
using Reveries.Infrastructure.Redis.Interfaces;
using Reveries.Infrastructure.Redis.Mappers;
using Reveries.Infrastructure.Redis.Models;
using StackExchange.Redis;

namespace Reveries.Infrastructure.Redis.Services;

public class BookCacheService : IBookCacheService
{
    private readonly IRedisCacheService _cache;
    private readonly ILogger<BookCacheService> _logger;
    
    private readonly StringComparer _titleComparer = StringComparer.OrdinalIgnoreCase;
    
    public BookCacheService(IRedisCacheService cacheService, ILogger<BookCacheService> logger)
    {
        _cache = cacheService;
        _logger = logger;
    }
    
    public async Task<Book?> GetBookByIsbnAsync(Isbn isbn, CancellationToken ct)
    {
        var key = CacheKeys.BookByIsbn(isbn.Value);

        var dto = await _cache.GetAsync<BookCacheDto>(key, ct);

        return dto?.ToDomain();
    }

    public async Task SetBookByIsbnAsync(Book book, CancellationToken ct)
    {
        if (book.Isbn13 == null && book.Isbn10 == null) return;
        
        var key = CacheKeys.BookByIsbn(book.Isbn13?.Value ?? book.Isbn10?.Value!);
        var dto = book.ToCacheDto();
        
        await _cache.SetAsync(key, dto, CacheDefaults.DefaultExpiration, ct);
    }

    public async Task RemoveBookByIsbnAsync(Isbn? isbn, CancellationToken ct)
    {
        if (isbn != null)
        {
            var key = CacheKeys.BookByIsbn(isbn.Value);
        
            await _cache.RemoveAsync(key, ct);
        }
    }
    
    public async Task<IReadOnlyList<Book>> GetBooksByIsbnsAsync(IEnumerable<Isbn> isbns, CancellationToken ct)
    {
        var distinctIsbns = isbns.Distinct().ToList();
        
        if (distinctIsbns.Count == 0)
            return ImmutableList<Book>.Empty;

        var batch = _cache.CreateBatch();

        var tasksByIsbn = distinctIsbns.ToDictionary(
            isbn => isbn,
            isbn => batch.StringGetAsync(CacheKeys.BookByIsbn(isbn.Value))
        );

        batch.Execute();
        await Task.WhenAll(tasksByIsbn.Values);

        var foundBooks = new List<Book>();

        foreach (var (isbn, task) in tasksByIsbn)
        {
            ct.ThrowIfCancellationRequested();

            RedisValue redisVal;
            try
            {
                redisVal = await task;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache batch read failed for ISBN {IsbnKey}.", isbn.Value);
                continue;
            }

            if (redisVal.IsNullOrEmpty)
                continue;

            BookCacheDto? dto;
            try
            {
                dto = JsonSerializer.Deserialize<BookCacheDto>((string)redisVal!);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize cached BookCacheDto for ISBN {IsbnKey}.", isbn.Value);
                continue;
            }

            var book = dto?.ToDomain();
            if (book is not null)
                foundBooks.Add(book);
        }

        _logger.LogDebug("Cache ISBN lookup completed. Requested {IsbnCount} Isbns, found {BooksCount} books in cache.", 
            distinctIsbns.Count, foundBooks.Count);
    
        return foundBooks.ToImmutableList();
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
        
        var isbnResults = new Dictionary<string, List<Isbn>>();
        foreach (var (title, task) in titleTasks)
        {
            var redisVal = await task;
            if (!redisVal.IsNullOrEmpty)
            {
                var json = (string)redisVal!;
                var isbnStrings = JsonSerializer.Deserialize<List<string>>(json) ?? [];

                var isbns = isbnStrings
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(Isbn.Create)
                    .ToList();

                isbnResults[title] = isbns;
            }
        }
        
        var allIsbns = isbnResults.Values.SelectMany(x => x).Distinct().ToList();
        
        if (allIsbns.Count == 0)
            return new List<Book>();
        
        var books = await GetBooksByIsbnsAsync(allIsbns, ct);
        
        _logger.LogDebug("Cache title lookup completed. Requested {TitleCount} titles, found {BooksCount} books in cache.", 
            distinctTitles.Count, 
            books.Count);
        
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