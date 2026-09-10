using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Enums;

namespace Reveries.Application.Books.Services;

/// <summary>
/// Merges the <see cref="BookCandidate"/> results for one book gathered from several external
/// sources, taking each field from the highest-priority source that provides a usable value.
/// </summary>
public static class BookCandidateMerger
{
    private static readonly BookSource[] Bibliographic = [BookSource.Isbndb, BookSource.GoogleBooks];
    private static readonly BookSource[] Descriptive = [BookSource.GoogleBooks, BookSource.Isbndb];

    public static BookCandidate? Merge(IReadOnlyDictionary<BookSource, BookCandidate> bySource)
    {
        if (bySource.Count == 0)
            return null;

        return new BookCandidate
        {
            Isbn = Pick(bySource, Bibliographic, c => c.Isbn),
            Title = PickString(bySource, Descriptive, c => c.Title) ?? string.Empty,
            Subtitle = PickString(bySource, Descriptive, c => c.Subtitle),
            Authors = PickList(bySource, Descriptive, c => c.Authors),
            Publisher = PickString(bySource, Bibliographic, c => c.Publisher),
            PrimaryGenres = PickList(bySource, Descriptive, c => c.PrimaryGenres),
            SecondaryGenres = PickList(bySource, Descriptive, c => c.SecondaryGenres),
            DeweyDecimals = PickList(bySource, Bibliographic, c => c.DeweyDecimals),
            Synopsis = PickString(bySource, Descriptive, c => c.Synopsis),
            Description = PickString(bySource, Descriptive, c => c.Description),
            Pages = PickPages(bySource, Bibliographic),
            PublicationDate = PickString(bySource, Descriptive, c => c.PublicationDate),
            Language = Pick(bySource, Bibliographic, c => c.Language),
            Format = PickFormat(bySource, Bibliographic),
            EditionStatement = PickString(bySource, Descriptive, c => c.EditionStatement),
            Cover = Cover.TryCreate(
                url: PickString(bySource, Bibliographic, c => c.Cover?.Url),
                thumbnailUrl: PickString(bySource, Bibliographic, c => c.Cover?.ThumbnailUrl)),
            Dimensions = MergeDimensions(bySource)
        };
    }

    private static BookDimensions? MergeDimensions(IReadOnlyDictionary<BookSource, BookCandidate> bySource)
    {
        return BookDimensions.Create(
            PickDecimal(bySource, Bibliographic, c => c.Dimensions?.HeightCm),
            PickDecimal(bySource, Bibliographic, c => c.Dimensions?.WidthCm),
            PickDecimal(bySource, Bibliographic, c => c.Dimensions?.ThicknessCm),
            PickDecimal(bySource, Bibliographic, c => c.Dimensions?.WeightG));
    }

    private static string? PickString(IReadOnlyDictionary<BookSource, BookCandidate> bySource, BookSource[] order, Func<BookCandidate, string?> selector)
    {
        foreach (var source in order)
        {
            if (bySource.TryGetValue(source, out var candidate))
            {
                var value = selector(candidate);
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }
        }

        return null;
    }

    private static IReadOnlyList<string> PickList(IReadOnlyDictionary<BookSource, BookCandidate> bySource, BookSource[] order, Func<BookCandidate, IReadOnlyList<string>> selector)
    {
        foreach (var source in order)
        {
            if (bySource.TryGetValue(source, out var candidate))
            {
                var value = selector(candidate);
                if (value.Count != 0)
                    return value;
            }
        }

        return [];
    }

    private static T? Pick<T>(IReadOnlyDictionary<BookSource, BookCandidate> bySource, BookSource[] order, Func<BookCandidate, T?> selector) where T : class
    {
        foreach (var source in order)
        {
            if (bySource.TryGetValue(source, out var candidate) && selector(candidate) is { } value)
                return value;
        }

        return null;
    }

    private static decimal? PickDecimal(IReadOnlyDictionary<BookSource, BookCandidate> bySource, BookSource[] order, Func<BookCandidate, decimal?> selector)
    {
        foreach (var source in order)
        {
            if (bySource.TryGetValue(source, out var candidate) && selector(candidate) is { } value)
                return value;
        }

        return null;
    }

    private static int? PickPages(IReadOnlyDictionary<BookSource, BookCandidate> bySource, BookSource[] order)
    {
        foreach (var source in order)
        {
            if (bySource.TryGetValue(source, out var candidate) && candidate.Pages is > 0)
                return candidate.Pages;
        }

        return null;
    }

    private static BookFormat PickFormat(IReadOnlyDictionary<BookSource, BookCandidate> bySource, BookSource[] order)
    {
        foreach (var source in order)
        {
            if (bySource.TryGetValue(source, out var candidate) && candidate.Format != BookFormat.Unknown)
                return candidate.Format;
        }

        return BookFormat.Unknown;
    }
}