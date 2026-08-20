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
            genres: cmd.Genres,
            deweyDecimals: cmd.DeweyDecimals,
            synopsis: cmd.Synopsis
        );

        var dataSourceParsed = Enum.TryParse<DataSource>(cmd.DataSource, true, out var ds);

        var edition = Edition.Create(
            workId: work.Id,
            isbn13: cmd.Isbn13?.Value,
            isbn10: cmd.Isbn10?.Value,
            publisher: cmd.Publisher,
            pages: cmd.Pages,
            publishDate: cmd.PublicationDate,
            languageIso639: cmd.Language,
            binding: cmd.Binding,
            editionStatement: cmd.Edition,
            imageThumbnail: cmd.ImageThumbnail,
            imageUrl: cmd.ImageUrl,
            msrp: cmd.Msrp,
            height: cmd.HeightCm,
            width: cmd.WidthCm,
            thickness: cmd.ThicknessCm,
            weight: cmd.WeightG,
            dataSource: dataSourceParsed ? ds : DataSource.Unknown
        );

        return (work, edition);
    }
}
