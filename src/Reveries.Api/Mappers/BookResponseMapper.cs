using Reveries.Application.Books.Models;
using Reveries.Api.Contracts.Books.Responses;

namespace Reveries.Api.Mappers;

public static class BookResponseMapper
{
    public static BookResponse ToResponse(this Book book)
    {
        return new BookResponse
        {
            BookId = book.BookId,
            Isbn10 = book.Isbn10,
            Isbn13 = book.Isbn13,
            Title = book.Title,
            Subtitle = book.Subtitle,
            Authors = book.Authors.ToList(),
            Publisher = book.Publisher,
            Language = book.Language,
            Pages = book.Pages,
            PublicationDate = book.PublicationDate,
            Synopsis = book.Synopsis,
            Description = book.Description,
            Format = book.Format,
            Edition = book.Edition,
            CoverImageUrl = book.CoverImageUrl,
            ImageThumbnailUrl = book.ImageThumbnailUrl,
            SaxoUrl = book.SaxoUrl,
            HeightCm = book.HeightCm,
            WidthCm = book.WidthCm,
            ThicknessCm = book.ThicknessCm,
            WeightG = book.WeightG,
            DeweyDecimals = book.DeweyDecimals.ToList(),
            PrimaryGenres = book.PrimaryGenres.ToList(),
            SecondaryGenres = book.SecondaryGenres.ToList()
        };
    }

    public static BookCollectionResponse ToCollectionResponse(this IEnumerable<Book> books)
    {
        return new BookCollectionResponse
        {
            Items = books.Select(book => book.ToResponse()).ToList()
        };
    }
}
