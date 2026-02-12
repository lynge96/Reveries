using Reveries.Application.Commands.CreateBook;
using Reveries.Application.Queries;
using Reveries.Core.Enums;
using Reveries.Core.Helpers;
using Reveries.Core.Models;

namespace Reveries.Application.Mappers;

public static class BookMapper
{
    public static Book ToDomain(this CreateBookCommand cmd)
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

    public static BookDetailsReadModel ToReadModel(this Book book)
    {
        return new BookDetailsReadModel
        {
            Id = book.Id.Value,
            Isbn10 = book.Isbn10?.Value,
            Isbn13 = book.Isbn13?.Value,
            Title = book.Title,
            Series = book.Series?.Name,
            NumberInSeries = book.SeriesNumber,
            Authors = book.Authors.Select(a => a.NormalizedName.ToTitleCase()).ToList(),
            Publisher = book.Publisher?.Name,
            Language = book.Language,
            Pages = book.Pages,
            PublicationDate = book.PublicationDate,
            Synopsis = book.Synopsis,
            Binding = book.Binding,
            Edition = book.Edition,
            ImageThumbnailUrl = book.ImageThumbnailUrl,
            CoverImageUrl = book.CoverImageUrl,
            Msrp = book.Msrp,
            IsRead = book.IsRead,
            WeightG = book.Dimensions?.WeightG,
            HeightCm = book.Dimensions?.HeightCm,
            WidthCm = book.Dimensions?.WidthCm,
            ThicknessCm = book.Dimensions?.ThicknessCm,
            DeweyDecimals = book.DeweyDecimals.Select(dd => dd.Code).ToList(),
            Genres = book.Genres.Select(g => g.Value).ToList(),
            DataSource = book.DataSource.ToString()
        };
    }
}