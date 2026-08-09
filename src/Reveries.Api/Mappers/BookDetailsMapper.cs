using Reveries.Contracts.Books.Dtos;
using Reveries.Contracts.Books.Responses;
using Reveries.Core.Models;

namespace Reveries.Api.Mappers;

public static class BookDetailsMapper
{
    public static BookDetailsDto ToDto(this Book book)
    {
        return new BookDetailsDto
        {
            BookId = book.Id.Value,
            Isbn10 = book.Isbn10?.Value,
            Isbn13 = book.Isbn13?.Value,
            Title = book.Title.Value,
            Series = book.Series?.Name,
            NumberInSeries = book.SeriesNumber,
            Authors = book.Authors.Select(a => a.NormalizedName).ToList(),
            Publisher = book.Publisher?.Name,
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
            HeightCm = book.Dimensions?.HeightCm,
            WidthCm = book.Dimensions?.WidthCm,
            ThicknessCm = book.Dimensions?.ThicknessCm,
            WeightG = book.Dimensions?.WeightG,
            DeweyDecimals = book.DeweyDecimals.Select(dd => dd.Code).ToList(),
            Genres = book.Genres.Select(g => g.Value).ToList(),
            DataSource = book.DataSource.ToString()
        };
    }

    public static BooksResponse ToResponse(this IEnumerable<Book> books)
    {
        var items = books
            .Select(b => b.ToDto())
            .ToList();

        return new BooksResponse
        {
            Items = items,
        };
    }
}