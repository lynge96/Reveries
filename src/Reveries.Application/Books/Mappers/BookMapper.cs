using Reveries.Application.Books.Commands.CreateBook;
using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Mappers;

public static class BookMapper
{
    public static (Work Work, Edition Edition) ToWorkAndEdition(this CreateBookCommand cmd)
    {
        var work = Work.Create(new WorkData(
            Title: cmd.Title,
            Subtitle: cmd.Subtitle,
            Authors: cmd.Authors,
            PrimaryGenres: cmd.PrimaryGenres,
            SecondaryGenres: cmd.SecondaryGenres,
            DeweyDecimals: cmd.DeweyDecimals,
            Synopsis: cmd.Synopsis,
            Description: cmd.Description
        ));

        var dimensions = BookDimensions.Create(cmd.HeightCm, cmd.WidthCm, cmd.ThicknessCm, cmd.WeightG);

        var edition = Edition.Create(new EditionData(
            WorkId: work.Id,
            Isbn13: cmd.Isbn?.Value13,
            Isbn10: cmd.Isbn?.Value10,
            Publisher: cmd.Publisher,
            Pages: cmd.Pages,
            PublishDate: cmd.PublicationDate,
            LanguageIso639: cmd.Language,
            Format: cmd.Format,
            EditionStatement: cmd.Edition,
            ImageThumbnail: cmd.ImageThumbnail,
            ImageUrl: cmd.ImageUrl,
            SaxoUrl: null,
            Dimensions: dimensions));

        return (work, edition);
    }
}
