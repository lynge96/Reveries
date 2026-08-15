using Reveries.Application.Books.Models;
using Reveries.Domain.Books;
using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Mappers;

/// <summary>
/// Temporary bridge that splits a <see cref="Book"/> (still produced by the external
/// integration and cache layers) into the composed <see cref="EditionWithWork"/> read-model.
/// Removed once integration and cache speak Work/Edition natively.
/// </summary>
public static class BookAdapter
{
    public static EditionWithWork ToEditionWithWork(this Book book)
    {
        var work = Work.Reconstitute(new WorkReconstitutionData(
            Id: Guid.NewGuid(),
            Title: book.Title.Value,
            Synopsis: book.Synopsis,
            SeriesNumber: book.SeriesNumber,
            Series: book.Series,
            Authors: book.Authors,
            Genres: book.Genres,
            DeweyDecimals: book.DeweyDecimals,
            DateCreated: book.DateCreated));

        var edition = Edition.Reconstitute(new EditionReconstitutionData(
            Id: book.Id.Value,
            WorkId: work.Id.Value,
            Isbn13: book.Isbn13?.Value,
            Isbn10: book.Isbn10?.Value,
            Pages: book.Pages,
            PublicationDate: book.PublicationDate,
            Language: book.Language,
            EditionStatement: book.Edition,
            Binding: book.Binding,
            ImageThumbnailUrl: book.ImageThumbnailUrl,
            CoverImageUrl: book.CoverImageUrl,
            Msrp: book.Msrp,
            Dimensions: book.Dimensions,
            DataSource: book.DataSource,
            Publisher: book.Publisher,
            DateCreated: book.DateCreated));

        return new EditionWithWork(edition, work);
    }
}