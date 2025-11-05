using Reveries.Contracts.DTOs;
using Reveries.Core.Enums;
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

    public static Book ToDomain(this BookDto bookDto)
    {
        var dataSourceParsed = Enum.TryParse<DataSource>(bookDto.DataSource, true, out var ds);

        return new Book
        {
            Isbn13 = bookDto.Isbn13,
            Isbn10 = bookDto.Isbn10,
            Title = bookDto.Title!,
            Authors = bookDto.Authors?.Select(Author.Create).ToList() ?? [],
            Publisher = bookDto.Publisher != null ? Publisher.Create(bookDto.Publisher) : null,
            Language = bookDto.Language,
            PublishDate = bookDto.PublicationDate.ParsePublishDate(),
            Pages = bookDto.Pages,
            Synopsis = bookDto.Synopsis,
            ImageThumbnail = bookDto.ImageThumbnail,
            ImageUrl = bookDto.ImageUrl,
            Msrp = bookDto.Msrp,
            Binding = bookDto.Binding,
            Edition = bookDto.Edition,
            Subjects = bookDto.Subjects?.Select(Subject.Create).ToList(),
            Series = bookDto.Series != null ? Series.Create(bookDto.Series) : null,
            SeriesNumber = bookDto.NumberInSeries,
            IsRead = bookDto.IsRead,
            Dimensions = new BookDimensions
            {
                HeightCm = bookDto.Dimensions?.HeightCm,
                ThicknessCm = bookDto.Dimensions?.ThicknessCm,
                WidthCm = bookDto.Dimensions?.WidthCm,
                WeightG = bookDto.Dimensions?.WeightG
            },
            DeweyDecimals = bookDto.DeweyDecimal?.Select(c => new DeweyDecimal { Code = c }).ToList(),
            DataSource = dataSourceParsed ? ds : DataSource.Unknown,
        };

    }
}