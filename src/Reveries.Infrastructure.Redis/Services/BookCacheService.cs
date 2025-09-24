using System.Collections.Immutable;
using System.Text.Json;
using Reveries.Application.Interfaces.Cache;
using Reveries.Core.Entities;
using Reveries.Core.Enums;
using Reveries.Infrastructure.Redis.Configuration;
using Reveries.Infrastructure.Redis.Helpers;

namespace Reveries.Infrastructure.Redis.Services;

public class BookCacheService : IBookCacheService
{
    private readonly ICacheService _cache;

    public BookCacheService(ICacheService cacheService)
    {
        _cache = cacheService;
    }
    
    public async Task<Book?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var key = CacheKeys.BookByIsbn(isbn);
        
        return await _cache.GetAsync<Book>(key, cancellationToken);
    }

    public async Task SetBookByIsbnAsync(Book book, CancellationToken cancellationToken = default)
    {
        if (book.Isbn13 == null && book.Isbn10 == null) return;
        
        var key = CacheKeys.BookByIsbn(book.Isbn13 ?? book.Isbn10!);
        
        await _cache.SetAsync(key, book, RedisSettings.Expiration, cancellationToken);
    }

    public async Task RemoveBookByIsbnAsync(string? isbn, CancellationToken cancellationToken = default)
    {
        if (isbn != null)
        {
            var key = CacheKeys.BookByIsbn(isbn);
        
            await _cache.RemoveAsync(key, cancellationToken);
        }
    }
    
    public async Task<IReadOnlyList<Book>> GetBooksByIsbnsAsync(IEnumerable<string> isbns, CancellationToken cancellationToken = default)
    {
        var tasks = isbns.Select(isbn => GetBookByIsbnAsync(isbn, cancellationToken));
        var books = await Task.WhenAll(tasks);
        
        var updatedBooks = books
            .Where(b => b is not null)
            .Select(b => b!.WithDataSource(b, DataSource.Cache))
            .ToImmutableList();

        return updatedBooks;
    }

    public async Task SetBooksByIsbnsAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default)
    {
        var tasks = books.Select(book => SetBookByIsbnAsync(book, cancellationToken));
        await Task.WhenAll(tasks);
    }

    public async Task<IReadOnlyList<Book>> GetBooksByTitlesAsync(IEnumerable<string> titles, CancellationToken cancellationToken = default)
    {
        var batch = _cache.CreateBatch();
        var titleTasks = titles.ToDictionary(
            title => title,
            title => batch.StringGetAsync(CacheKeys.BookIsbnsByTitle(title))
        );
        batch.Execute();
        await Task.WhenAll(titleTasks.Values);
        
        var isbnResults = new Dictionary<string, List<string>>();
        foreach (var kvp in titleTasks)
        {
            var redisVal = kvp.Value.Result;
            if (!redisVal.IsNullOrEmpty)
            {
                var isbns = JsonSerializer.Deserialize<List<string>>(redisVal!) ?? new();
                isbnResults[kvp.Key] = isbns;
            }
        }
        
        var allIsbns = isbnResults.Values.SelectMany(x => x).Distinct().ToList();
        
        if (allIsbns.Count == 0)
            return new List<Book>();
        
        var books = await GetBooksByIsbnsAsync(allIsbns, cancellationToken);
        return books.ToList();
    }

    public async Task SetIsbnsByTitleAsync(IEnumerable<string> titles, IEnumerable<string> isbns, CancellationToken cancellationToken = default)
    {
        var batch = _cache.CreateBatch();

        var serialized = JsonSerializer.Serialize(isbns.ToList());

        var tasks = titles.Select(title =>
        {
            var key = CacheKeys.BookIsbnsByTitle(title);
            return batch.StringSetAsync(key, serialized, RedisSettings.Expiration);
        }).ToList();
        
        batch.Execute();
        await Task.WhenAll(tasks);
    }
}