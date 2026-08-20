using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Enums;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Services;

/// <summary>
/// Field-by-field merge of two <see cref="EditionWithWork"/> candidates for the same book
/// (typically one from ISBNDB, one from Google Books), preferring the richer source per field.
/// </summary>
public static class EditionWithWorkMerger
{
    public static EditionWithWork? Merge(EditionWithWork? isbndb, EditionWithWork? google)
    {
        if (isbndb is null && google is null)
            return null;
        if (isbndb is null)
            return google;
        if (google is null)
            return isbndb;

        var iw = isbndb.Work;
        var gw = google.Work;
        var ie = isbndb.Edition;
        var ge = google.Edition;

        var work = Work.Reconstitute(new WorkReconstitutionData(
            Id: iw.Id.Value,
            Title: Prefer(gw.Title.Value, iw.Title.Value) ?? string.Empty,
            Synopsis: (gw.Synopsis ?? iw.Synopsis)?.Text,
            SeriesNumber: iw.SeriesPlacement?.Number,
            Series: iw.SeriesPlacement?.Series,
            Authors: gw.Authors.Count != 0 ? gw.Authors : iw.Authors,
            PrimaryGenres: gw.Genres.Primary.Count != 0 ? gw.Genres.Primary : iw.Genres.Primary,
            SecondaryGenres: gw.Genres.Secondary.Count != 0 ? gw.Genres.Secondary : iw.Genres.Secondary,
            DeweyDecimals: iw.DeweyDecimals));

        var edition = Edition.Reconstitute(new EditionReconstitutionData(
            Id: ie.Id.Value,
            WorkId: work.Id.Value,
            Isbn13: ie.Isbn13?.Value ?? ge.Isbn13?.Value,
            Isbn10: ie.Isbn10?.Value ?? ge.Isbn10?.Value,
            Pages: ie.Pages > 0 ? ie.Pages : ge.Pages,
            PublicationDate: ge.PublicationDate ?? ie.PublicationDate,
            Language: Prefer(ie.Language, ge.Language),
            EditionStatement: Prefer(ge.EditionStatement, ie.EditionStatement),
            Binding: Prefer(ie.Binding, ge.Binding),
            ImageThumbnailUrl: ge.ImageThumbnailUrl ?? ie.ImageThumbnailUrl,
            CoverImageUrl: ie.ImageThumbnailUrl ?? ie.CoverImageUrl ?? ge.CoverImageUrl,
            Msrp: ie.Msrp,
            Dimensions: MergeDimensions(ie.Dimensions, ge.Dimensions),
            DataSource: DataSource.CombinedBookApi,
            Publisher: ie.Publisher ?? ge.Publisher));

        return new EditionWithWork(edition, work);
    }

    public static string? GetIsbnKey(EditionWithWork item)
    {
        if (!string.IsNullOrWhiteSpace(item.Edition.Isbn13?.Value))
            return item.Edition.Isbn13!.Value;
        if (!string.IsNullOrWhiteSpace(item.Edition.Isbn10?.Value))
            return item.Edition.Isbn10!.Value;
        return null;
    }

    private static string? Prefer(params string?[] values)
        => values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

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