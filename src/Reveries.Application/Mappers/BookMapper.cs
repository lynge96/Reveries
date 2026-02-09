using Reveries.Application.Commands.CreateBook;
using Reveries.Core.Enums;
using Reveries.Core.Models;

namespace Reveries.Application.Mappers;

public static class BookMapper
{
    public static Book MapToDomain(this CreateBookCommand cmd)
    {
        var dataSourceParsed = Enum.TryParse<DataSource>(cmd.DataSource, true, out var ds);

        return Book.Create(
            isbn10: cmd.Isbn10?.Value,
            isbn13: cmd.Isbn13?.Value,
            title: cmd.Title,
            pages: cmd.Pages,
            publishDate: cmd.PublicationDate,
            languageIso639: cmd.Language,
            synopsis: cmd.Synopsis,
            imageThumbnail: cmd.ImageThumbnail,
            imageUrl: cmd.ImageUrl,
            msrp: cmd.Msrp,
            binding: cmd.Binding,
            edition: cmd.Edition,
            dataSource: dataSourceParsed ? ds : DataSource.Unknown,
            publisher: cmd.Publisher,
            weight: cmd.WeightG,
            height: cmd.HeightCm,
            width: cmd.WidthCm,
            thickness: cmd.ThicknessCm,
            authors: cmd.Authors,
            subjects: cmd.Genres,
            deweyDecimals: cmd.DeweyDecimals
        );
    }
}