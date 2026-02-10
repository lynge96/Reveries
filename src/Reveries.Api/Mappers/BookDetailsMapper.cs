using Reveries.Contracts.Books;
using Reveries.Core.Models;

namespace Reveries.Api.Mappers;

public static class BookDetailsMapper
{
    public static BookDetailsDto ToDto(this Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        return new BookDetailsDto
        {
            Id = book.Id.Value,
            Isbn10 = book.Isbn10?.Value,
            Isbn13 = book.Isbn13?.Value,
            Title = book.Title,
            Series = book.Series?.Name,
            NumberInSeries = book.SeriesNumber,
            Authors = book.Authors.Select(a => a.NormalizedName).ToList(),
            Publisher = book.Publisher?.Name,
            Language = book.Language,
            Pages = book.Pages,
            PublicationDate = book.PublishDate,
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
}