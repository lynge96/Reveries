using Reveries.Application.Books.Commands.CreateBook;
using Reveries.Domain.Editions;
using Reveries.Domain.Enums;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Mappers;

public static class BookMapper
{
    public static (Work Work, Edition Edition) ToWorkAndEdition(this CreateBookCommand cmd)
    {
        var work = Work.Create(
            title: cmd.Title,
            authors: cmd.Authors,
            primaryGenres: cmd.PrimaryGenres,
            secondaryGenres: cmd.SecondaryGenres,
            deweyDecimals: cmd.DeweyDecimals,
            synopsis: cmd.Synopsis
        );

        var dataSourceParsed = Enum.TryParse<DataSource>(cmd.DataSource, true, out var ds);

        var dimensions = BookDimensions.Create(cmd.HeightCm, cmd.WidthCm, cmd.ThicknessCm, cmd.WeightG);

        var edition = Edition.Create(new EditionData(
            WorkId: work.Id,
            Isbn13: cmd.Isbn?.Value13,
            Isbn10: cmd.Isbn?.Value10,
            Publisher: cmd.Publisher,
            Pages: cmd.Pages,
            PublishDate: cmd.PublicationDate,
            LanguageIso639: cmd.Language,
            Binding: cmd.Binding,
            EditionStatement: cmd.Edition,
            ImageThumbnail: cmd.ImageThumbnail,
            ImageUrl: cmd.ImageUrl,
            SaxoUrl: null,
            Msrp: cmd.Msrp,
            Dimensions: dimensions,
            DataSource: dataSourceParsed ? ds : DataSource.Unknown));

        return (work, edition);
    }
}
