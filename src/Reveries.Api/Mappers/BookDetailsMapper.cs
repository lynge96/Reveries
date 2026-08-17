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
            Isbn10 = edition.Isbn10?.Value,
            Isbn13 = edition.Isbn13?.Value,
            Title = work.Title.Value,
            Series = work.SeriesPlacement?.Series.Name,
            NumberInSeries = work.SeriesPlacement?.Number,
            Authors = work.Authors.Select(a => a.NormalizedName).ToList(),
            Publisher = edition.Publisher?.Name,
            Language = edition.Language,
            Pages = edition.Pages,
            PublicationDate = edition.PublicationDate,
            Synopsis = work.Synopsis?.Value,
            Binding = edition.Binding,
            Edition = edition.EditionStatement,
            CoverImageUrl = edition.CoverImageUrl,
            ImageThumbnailUrl = edition.ImageThumbnailUrl,
            Msrp = edition.Msrp,
            HeightCm = edition.Dimensions?.HeightCm,
            WidthCm = edition.Dimensions?.WidthCm,
            ThicknessCm = edition.Dimensions?.ThicknessCm,
            WeightG = edition.Dimensions?.WeightG,
            DeweyDecimals = work.DeweyDecimals.Select(dd => dd.Code).ToList(),
            Genres = work.Genres.Select(g => g.Value).ToList(),
            DataSource = edition.DataSource.ToString()
        };
    }

    public static BooksResponse ToResponse(this IEnumerable<EditionWithWork> items)
    {
        return new BooksResponse
        {
            Items = items.Select(i => i.ToDto()).ToList()
        };
    }
}