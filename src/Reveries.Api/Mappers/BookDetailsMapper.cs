using Reveries.Application.Books.Models;
using Reveries.Contracts.Books.Dtos;
using Reveries.Contracts.Books.Responses;

namespace Reveries.Api.Mappers;

public static class BookDetailsMapper
{
    public static BookDetailsDto ToDto(this BookCandidate book)
    {
        return new BookDetailsDto
        {
            BookId = Guid.Empty,
            Isbn10 = book.Isbn?.Value10,
            Isbn13 = book.Isbn?.Value13,
            Title = book.Title,
            Subtitle = book.Subtitle,
            Authors = book.Authors.ToList(),
            Publisher = book.Publisher,
            Language = book.Language?.DisplayName,
            Pages = book.Pages,
            PublicationDate = book.PublicationDate,
            Synopsis = book.Synopsis,
            Description = book.Description,
            Format = book.Format.ToString(),
            Edition = book.EditionStatement,
            CoverImageUrl = book.Cover?.Url,
            ImageThumbnailUrl = book.Cover?.ThumbnailUrl,
            HeightCm = book.Dimensions?.HeightCm,
            WidthCm = book.Dimensions?.WidthCm,
            ThicknessCm = book.Dimensions?.ThicknessCm,
            WeightG = book.Dimensions?.WeightG,
            DeweyDecimals = book.DeweyDecimals.ToList(),
            PrimaryGenres = book.PrimaryGenres.ToList(),
            SecondaryGenres = book.SecondaryGenres.ToList()
        };
    }

    public static BooksResponse ToResponse(this IEnumerable<BookCandidate> books)
    {
        return new BooksResponse
        {
            Items = books.Select(book => book.ToDto()).ToList()
        };
    }

    public static BookDetailsDto ToDto(this BookDetails book)
    {
        return new BookDetailsDto
        {
            BookId = book.BookId,
            Isbn10 = book.Isbn10,
            Isbn13 = book.Isbn13,
            Title = book.Title,
            Subtitle = book.Subtitle,
            Series = book.Series,
            NumberInSeries = book.NumberInSeries,
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
            HeightCm = book.HeightCm,
            WidthCm = book.WidthCm,
            ThicknessCm = book.ThicknessCm,
            WeightG = book.WeightG,
            DeweyDecimals = book.DeweyDecimals.ToList(),
            PrimaryGenres = book.PrimaryGenres.ToList(),
            SecondaryGenres = book.SecondaryGenres.ToList()
        };
    }

    public static BooksResponse ToResponse(this IEnumerable<BookDetails> books)
    {
        return new BooksResponse
        {
            Items = books.Select(book => book.ToDto()).ToList()
        };
    }
}