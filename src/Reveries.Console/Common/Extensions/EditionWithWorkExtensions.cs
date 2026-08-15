using Reveries.Application.Books.Models;
using Reveries.Domain.Books;

namespace Reveries.Console.Common.Extensions;

/// <summary>
/// Temporary bridge that flattens the <see cref="EditionWithWork"/> read-model back into a
/// <see cref="Book"/> so the console's existing Book-based display/selection/save layer keeps
/// working until the console client is migrated to Work/Edition.
/// </summary>
public static class EditionWithWorkExtensions
{
    public static Book ToBook(this EditionWithWork item)
    {
        var e = item.Edition;
        var w = item.Work;

        return Book.Reconstitute(new BookReconstitutionData(
            Id: e.Id.Value,
            Title: w.Title.Value,
            Isbn13: e.Isbn13?.Value,
            Isbn10: e.Isbn10?.Value,
            Pages: e.Pages,
            PublicationDate: e.PublicationDate,
            Language: e.Language,
            Synopsis: w.Synopsis,
            ImageThumbnailUrl: e.ImageThumbnailUrl,
            CoverImageUrl: e.CoverImageUrl,
            Msrp: e.Msrp,
            Binding: e.Binding,
            Edition: e.EditionStatement,
            SeriesNumber: w.SeriesNumber,
            DataSource: e.DataSource,
            Publisher: e.Publisher,
            Series: w.Series,
            Dimensions: e.Dimensions,
            Authors: w.Authors,
            Genres: w.Genres,
            DeweyDecimals: w.DeweyDecimals,
            DateCreated: e.DateCreated));
    }

    public static List<Book> ToBooks(this IEnumerable<EditionWithWork> items)
        => items.Select(i => i.ToBook()).ToList();
}