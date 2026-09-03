using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Enums;

namespace Reveries.Application.Books.Services;

/// <summary>
/// Field-by-field merge of two <see cref="BookCandidate"/> results for the same book
/// (typically one from ISBNDB, one from Google Books), preferring the richer source per field.
/// </summary>
public static class BookCandidateMerger
{
    public static BookCandidate? Merge(BookCandidate? isbndb, BookCandidate? google)
    {
        if (isbndb is null && google is null)
            return null;
        if (isbndb is null)
            return google;
        if (google is null)
            return isbndb;

        return new BookCandidate
        {
            Isbn = isbndb.Isbn ?? google.Isbn,
            Title = Prefer(google.Title, isbndb.Title) ?? string.Empty,
            Subtitle = Prefer(google.Subtitle, isbndb.Subtitle),
            Authors = Prefer(google.Authors, isbndb.Authors),
            Publisher = Prefer(isbndb.Publisher, google.Publisher),
            PrimaryGenres = Prefer(google.PrimaryGenres, isbndb.PrimaryGenres),
            SecondaryGenres = Prefer(google.SecondaryGenres, isbndb.SecondaryGenres),
            DeweyDecimals = isbndb.DeweyDecimals,
            Synopsis = Prefer(google.Synopsis, isbndb.Synopsis),
            Description = Prefer(google.Description, isbndb.Description),
            Pages = isbndb.Pages > 0 ? isbndb.Pages : google.Pages,
            PublicationDate = Prefer(google.PublicationDate, isbndb.PublicationDate),
            Language = isbndb.Language ?? google.Language,
            Format = PreferFormat(isbndb.Format, google.Format),
            EditionStatement = Prefer(google.EditionStatement, isbndb.EditionStatement),
            Cover = Cover.TryCreate(
                url: isbndb.Cover?.Url ?? google.Cover?.Url,
                thumbnailUrl: isbndb.Cover?.ThumbnailUrl ?? google.Cover?.ThumbnailUrl),
            Dimensions = MergeDimensions(isbndb.Dimensions, google.Dimensions)
        };
    }

    public static string? GetIsbnKey(BookCandidate candidate)
    {
        if (!string.IsNullOrWhiteSpace(candidate.Isbn?.Value13))
            return candidate.Isbn.Value13;
        if (!string.IsNullOrWhiteSpace(candidate.Isbn?.Value10))
            return candidate.Isbn.Value10;
        return null;
    }

    private static string? Prefer(params string?[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
    }

    private static IReadOnlyList<string> Prefer(IReadOnlyList<string> first, IReadOnlyList<string> second)
    {
        return first.Count != 0 ? first : second;
    }

    private static BookFormat PreferFormat(BookFormat first, BookFormat second)
    {
        return first != BookFormat.Unknown ? first : second;
    }

    private static BookDimensions? MergeDimensions(BookDimensions? isbndb, BookDimensions? google)
    {
        if (isbndb is null && google is null)
            return null;

        return BookDimensions.Create(
            isbndb?.HeightCm ?? google?.HeightCm,
            isbndb?.WidthCm ?? google?.WidthCm,
            isbndb?.ThicknessCm ?? google?.ThicknessCm,
            isbndb?.WeightG);
    }
}
