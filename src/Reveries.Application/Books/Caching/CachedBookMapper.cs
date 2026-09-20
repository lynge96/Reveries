using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Caching;

/// <summary>
/// Maps between the rich <see cref="BookCandidate"/> read-model and the flat, serializable
/// <see cref="CachedBook"/> used in the external-lookup cache. Reconstruction goes through the
/// public value-object factories, so cached scalars are re-validated (idempotently) on the way back.
/// </summary>
public static class CachedBookMapper
{
    public static CachedBook ToCached(BookCandidate candidate)
    {
        return new CachedBook
        {
            Isbn13 = candidate.Isbn?.Value13,
            Isbn10 = candidate.Isbn?.Value10,
            Title = candidate.Title,
            Subtitle = candidate.Subtitle,
            Authors = candidate.Authors,
            Publisher = candidate.Publisher,
            PrimaryGenres = candidate.PrimaryGenres,
            SecondaryGenres = candidate.SecondaryGenres,
            DeweyDecimals = candidate.DeweyDecimals,
            Synopsis = candidate.Synopsis,
            Description = candidate.Description,
            Pages = candidate.Pages,
            PublicationDate = candidate.PublicationDate,
            LanguageCode = candidate.Language?.Value,
            Format = candidate.Format,
            EditionStatement = candidate.EditionStatement,
            CoverUrl = candidate.Cover?.Url,
            CoverThumbnailUrl = candidate.Cover?.ThumbnailUrl,
            HeightCm = candidate.Dimensions?.HeightCm,
            WidthCm = candidate.Dimensions?.WidthCm,
            ThicknessCm = candidate.Dimensions?.ThicknessCm,
            WeightG = candidate.Dimensions?.WeightG
        };
    }

    public static BookCandidate ToCandidate(CachedBook cached)
    {
        return new BookCandidate
        {
            Isbn = ResolveIsbn(cached.Isbn13, cached.Isbn10),
            Title = cached.Title,
            Subtitle = cached.Subtitle,
            Authors = cached.Authors,
            Publisher = cached.Publisher,
            PrimaryGenres = cached.PrimaryGenres,
            SecondaryGenres = cached.SecondaryGenres,
            DeweyDecimals = cached.DeweyDecimals,
            Synopsis = cached.Synopsis,
            Description = cached.Description,
            Pages = cached.Pages,
            PublicationDate = cached.PublicationDate,
            Language = Language.TryCreate(cached.LanguageCode),
            Format = cached.Format,
            EditionStatement = cached.EditionStatement,
            Cover = Cover.TryCreate(cached.CoverUrl, cached.CoverThumbnailUrl),
            Dimensions = BookDimensions.Create(cached.HeightCm, cached.WidthCm, cached.ThicknessCm, cached.WeightG)
        };
    }

    private static Isbn? ResolveIsbn(string? isbn13, string? isbn10)
    {
        if (!string.IsNullOrWhiteSpace(isbn13))
            return Isbn.Create(isbn13);

        return string.IsNullOrWhiteSpace(isbn10) ? null : Isbn.Create(isbn10);
    }
}
