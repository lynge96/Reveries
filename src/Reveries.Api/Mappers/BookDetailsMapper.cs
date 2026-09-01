using Reveries.Application.Books.Models;
using Reveries.Contracts.Books.Dtos;
using Reveries.Contracts.Books.Responses;

namespace Reveries.Api.Mappers;

public static class BookDetailsMapper
{
    public static BookDetailsDto ToDto(this EditionWithWork item)
    {
        var edition = item.Edition;
        var work = item.Work;

        return new BookDetailsDto
        {
            BookId = edition.Id.Value,
            Isbn10 = edition.Isbn?.Value10,
            Isbn13 = edition.Isbn?.Value13,
            Title = work.Title.Text,
            Subtitle = work.Subtitle,
            Series = work.SeriesPlacement?.Series.Name,
            NumberInSeries = work.SeriesPlacement?.Number,
            Authors = work.Authors.Select(a => a.Name).ToList(),
            Publisher = edition.Publisher?.Name,
            Language = edition.Language?.DisplayName,
            Pages = edition.Pages,
            PublicationDate = edition.PublicationDate?.Value,
            Synopsis = work.Synopsis?.Text,
            Description = work.Description?.Text,
            Format = edition.Format.ToString(),
            Edition = edition.EditionDescription,
            CoverImageUrl = edition.Cover?.Url,
            ImageThumbnailUrl = edition.Cover?.ThumbnailUrl,
            HeightCm = edition.Dimensions?.HeightCm,
            WidthCm = edition.Dimensions?.WidthCm,
            ThicknessCm = edition.Dimensions?.ThicknessCm,
            WeightG = edition.Dimensions?.WeightG,
            DeweyDecimals = work.DeweyDecimals.Select(dd => dd.Code).ToList(),
            PrimaryGenres = work.Genres.Primary.Select(g => g.Name).ToList(),
            SecondaryGenres = work.Genres.Secondary.Select(g => g.Name).ToList()
        };
    }

    public static BooksResponse ToResponse(this IEnumerable<EditionWithWork> items)
    {
        return new BooksResponse
        {
            Items = items.Select(i => i.ToDto()).ToList()
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