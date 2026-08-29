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
}