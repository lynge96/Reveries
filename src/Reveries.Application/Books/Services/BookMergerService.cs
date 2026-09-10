using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Services;

public class BookMergerService : IBookMergerService
{
    private readonly ILogger<BookMergerService> _logger;

    public BookMergerService(ILogger<BookMergerService> logger)
    {
        _logger = logger;
    }

    public List<BookCandidate> AggregateBooksByIsbns(IReadOnlyList<Isbn> isbns, IReadOnlyList<SourcedBooks> sources)
    {
        if (isbns.Count == 0 || sources.Count == 0)
            return [];

        var indexBySource = sources.ToDictionary(s => s.Source, s => BuildIsbnDictionary(s.Books));

        var merged = isbns
            .Select(isbn => MergeForIsbn(isbn, indexBySource))
            .OfType<BookCandidate>()
            .ToList();

        _logger.LogDebug("Aggregated {MergedCount} books from {IsbnCount} ISBNs across {SourceCount} source(s).",
            merged.Count, isbns.Count, sources.Count);

        return merged;
    }

    private static BookCandidate? MergeForIsbn(Isbn isbn, IReadOnlyDictionary<BookSource, Dictionary<string, BookCandidate>> indexBySource)
    {
        var bySource = new Dictionary<BookSource, BookCandidate>();

        foreach (var (source, index) in indexBySource)
        {
            if (index.TryGetValue(isbn.Value13, out var candidate))
                bySource[source] = candidate;
        }

        return BookCandidateMerger.Merge(bySource);
    }

    private static Dictionary<string, BookCandidate> BuildIsbnDictionary(IEnumerable<BookCandidate> items)
    {
        return items
            .SelectMany(x => new[]
            {
                (isbn: x.Isbn?.Value10, item: x),
                (isbn: x.Isbn?.Value13, item: x)
            })
            .Where(t => t.isbn is not null)
            .GroupBy(t => t.isbn!)
            .ToDictionary(g => g.Key, g => g.First().item);
    }
}