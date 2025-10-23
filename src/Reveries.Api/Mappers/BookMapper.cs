using Reveries.Contracts.Books;
using Reveries.Core.Helpers;
using Reveries.Core.Models;

namespace Reveries.Api.Mappers;

public static class BookMapper
{
    public static BookDto ToDto(this Book book)
    {
        return new BookDto
        {
            Isbn10 = book.Isbn10,
            Isbn13 = book.Isbn13,
            Title = book.Title,
            Authors = book.Authors.Select(a => a.NormalizedName.ToTitleCase()).ToList(),
            Publisher = book.Publisher?.Name,
            Language = book.Language,
            PublicationDate = book.PublishDateFormatted,
            Pages = book.Pages,
            Synopsis = book.Synopsis,
            ImageUrl = book.ImageUrl,
            ImageThumbnail = book.ImageThumbnail,
            Msrp = book.Msrp,
            Binding = book.Binding,
            Edition = book.Edition,
            Subjects = book.Subjects?.Select(s => s.Genre).ToList() ?? new List<string>(),
            Series = book.Series?.Name,
            NumberInSeries = book.SeriesNumber,
            IsRead = book.IsRead,
            Dimensions = new DimensionsDto
            {
                WeightG = book.Dimensions?.WeightG,
                HeightCm = book.Dimensions?.HeightCm,
                WidthCm = book.Dimensions?.WidthCm,
                ThicknessCm = book.Dimensions?.ThicknessCm,
            },
            DeweyDecimal = book.DeweyDecimals?.Select(d => d.Code).ToList() ?? new List<string>(),
            DataSource = book.DataSource.ToString()
        };
    }
}