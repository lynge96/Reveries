using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Persistence.Rows;

namespace Reveries.Persistence.Mappers;

public static class BookMappingExtensions
{
    public static Book ToBook(this BookRow row)
    {
        return new Book
        {
            BookId = row.Id,
            Isbn10 = row.Isbn10,
            Isbn13 = row.Isbn13,
            Title = row.Title,
            Subtitle = row.Subtitle,
            Authors = row.Authors,
            Publisher = row.Publisher,
            Language = Language.TryCreate(row.Language)?.DisplayName,
            Pages = row.PageCount,
            PublicationDate = row.PublicationDate,
            Synopsis = row.Synopsis,
            Description = row.Description,
            Format = row.Format,
            Edition = row.EditionStatement,
            CoverImageUrl = row.ImageUrl,
            ImageThumbnailUrl = row.ImageThumbnail,
            SaxoUrl = row.SaxoUrl,
            HeightCm = row.HeightCm,
            WidthCm = row.WidthCm,
            ThicknessCm = row.ThicknessCm,
            WeightG = row.WeightG,
            DeweyDecimals = row.DeweyCodes,
            PrimaryGenres = row.PrimaryGenres,
            SecondaryGenres = row.SecondaryGenres
        };
    }
}