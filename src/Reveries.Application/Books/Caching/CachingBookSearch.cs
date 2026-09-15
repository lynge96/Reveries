using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Caching;
using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Caching;

/// <summary>
/// Caching decorator over an <see cref="IBookSearch"/> source. Caches each source's result per ISBN
/// in <see cref="HybridCache"/> (in-memory), so repeated scans of the same book do not re-hit the
/// external API. Empty results ("book not found") are cached too; thrown failures are not.
/// </summary>
public sealed class CachingBookSearch : IBookSearch
{
    private readonly IBookSearch _inner;
    private readonly HybridCache _cache;
    private readonly HybridCacheEntryOptions _entryOptions;

    public CachingBookSearch(IBookSearch inner, HybridCache cache, IOptions<CacheSettings> settings)
    {
        _inner = inner;
        _cache = cache;

        var ttl = TimeSpan.FromHours(settings.Value.IsbnLookupTtlHours);
        _entryOptions = new HybridCacheEntryOptions
        {
            Expiration = ttl,
            LocalCacheExpiration = ttl
        };
    }

    public BookSource Source => _inner.Source;

    public async Task<IReadOnlyList<BookCandidate>?> GetBooksByIsbnsAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct = default)
    {
        if (isbns.Count == 0)
            return [];

        var cachedBooks = await Task.WhenAll(isbns.Select(isbn => GetSingleAsync(isbn, ct).AsTask()));

        var found = cachedBooks
            .OfType<CachedBook>()
            .Select(CachedBookMapper.ToCandidate)
            .ToList();

        return found.Count == 0 ? null : found;
    }

    private ValueTask<CachedBook?> GetSingleAsync(Isbn isbn, CancellationToken ct)
    {
        return _cache.GetOrCreateAsync(
            CacheKey(isbn),
            (Inner: _inner, Isbn: isbn),
            static async (state, token) =>
            {
                var books = await state.Inner.GetBooksByIsbnsAsync([state.Isbn], token);
                var candidate = books?.FirstOrDefault();

                return candidate is null ? null : CachedBookMapper.ToCached(candidate);
            },
            _entryOptions,
            cancellationToken: ct);
    }

    private string CacheKey(Isbn isbn)
    {
        return $"booksearch:{Source}:{isbn.Value13}";
    }
}
