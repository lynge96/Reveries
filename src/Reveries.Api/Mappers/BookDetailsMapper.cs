using Reveries.Application.Queries;
using Reveries.Contracts.Books;

namespace Reveries.Api.Mappers;

public static class BookDetailsMapper
{
    public static BookDetailsDto ToDto(this BookDetailsReadModel book)
    {
        ArgumentNullException.ThrowIfNull(book);

        return new BookDetailsDto
        {
            Id = book.Id,
            Isbn10 = book.Isbn10,
            Isbn13 = book.Isbn13,
            Title = book.Title,
            Series = book.Series,
            NumberInSeries = book.NumberInSeries,
            Authors = book.Authors,
            Publisher = book.Publisher,
            Language = book.Language,
            Pages = book.Pages,
            PublicationDate = book.PublicationDate,
            Synopsis = book.Synopsis,
            Binding = book.Binding,
            Edition = book.Edition,
            CoverImageUrl = book.CoverImageUrl,
            ImageThumbnailUrl = book.ImageThumbnailUrl,
            Msrp = book.Msrp,
            IsRead = book.IsRead,
            HeightCm = book.HeightCm,
            WidthCm = book.WidthCm,
            ThicknessCm = book.ThicknessCm,
            WeightG = book.WeightG,
            DeweyDecimals = book.DeweyDecimals,
            Genres = book.Genres,
            DataSource = book.DataSource
        };
    }
}