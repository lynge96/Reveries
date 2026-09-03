using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Services;

public class BookMergerService : IBookMergerService
{
    private readonly ILogger<BookMergerService> _logger;

    public BookMergerService(ILogger<BookMergerService> logger)
    {
        _logger = logger;
    }

    public List<EditionWithWork> AggregateBooksByIsbnsAsync(
        IReadOnlyList<Isbn> isbns,
        IReadOnlyList<EditionWithWork>? isbndbBooks,
        IReadOnlyList<EditionWithWork>? googleBooks)
    {
        if (isbns.Count == 0 || (isbndbBooks is null && googleBooks is null))
            return [];

        var googleDict = BuildIsbnDictionary(googleBooks ?? []);
        var isbndbDict = BuildIsbnDictionary(isbndbBooks ?? []);

        var merged = isbns
            .Select(isbn =>
            {
                isbndbDict.TryGetValue(isbn.Value13, out var isbndbBook);
                googleDict.TryGetValue(isbn.Value13, out var googleBook);

                return EditionWithWorkMerger.Merge(isbndbBook, googleBook);
            })
            .OfType<EditionWithWork>()
            .ToList();

        _logger.LogDebug("Aggregated {MergedCount} books from {IsbnCount} ISBNs.", merged.Count, isbns.Count);
        return merged;
    }

    public List<EditionWithWork> AggregateBooksByTitlesAsync(
        IReadOnlyList<Title> titles,
        IReadOnlyList<EditionWithWork>? isbndbBooks,
        IReadOnlyList<EditionWithWork>? googleBooks)
    {
        if (titles.Count == 0)
            return [];

        var mergedByIsbn = MergeDictionaries(googleBooks ?? [], isbndbBooks ?? []);

        var merged = mergedByIsbn.Values
            .Where(x =>
                !string.IsNullOrWhiteSpace(x.Edition.Isbn?.Value13) ||
                !string.IsNullOrWhiteSpace(x.Edition.Isbn?.Value10))
            .ToList();

        _logger.LogDebug("Aggregated {MergedCount} books from {TitleCount} titles.", merged.Count, titles.Count);
        return merged;
    }

    private static Dictionary<string, EditionWithWork> BuildIsbnDictionary(IEnumerable<EditionWithWork> items)
    {
        return items
            .SelectMany(x => new[]
            {
                (isbn: x.Edition.Isbn?.Value10, item: x),
                (isbn: x.Edition.Isbn?.Value13, item: x)
            })
            .Where(t => t.isbn is not null)
            .GroupBy(t => t.isbn!)
            .ToDictionary(g => g.Key, g => g.First().item);
    }

    private static Dictionary<string, EditionWithWork> MergeDictionaries(IEnumerable<EditionWithWork> primary, IEnumerable<EditionWithWork> secondary)
    {
        var secondaryDict = secondary
            .Select(x => new { Item = x, Key = EditionWithWorkMerger.GetIsbnKey(x) })
            .Where(x => x.Key is not null)
            .GroupBy(x => x.Key!)
            .ToDictionary(g => g.Key, g => g.First().Item);

        var mergedByIsbn = new Dictionary<string, EditionWithWork>();

        foreach (var primaryItem in primary)
        {
            var key = EditionWithWorkMerger.GetIsbnKey(primaryItem);
            if (key is null)
                continue;

            if (secondaryDict.TryGetValue(key, out var secondaryItem))
            {
                mergedByIsbn[key] = EditionWithWorkMerger.Merge(secondaryItem, primaryItem)!;
                secondaryDict.Remove(key);
            }
            else
            {
                mergedByIsbn[key] = primaryItem;
            }
        }

        foreach (var (key, item) in secondaryDict)
            mergedByIsbn.TryAdd(key, item);

        return mergedByIsbn;
    }
}