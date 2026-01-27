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
            Id = book.Id,
            Isbn10 = book.Isbn10,
            Isbn13 = book.Isbn13,
            Title = book.Title,
            Authors = book.Authors.Select(a => a.NormalizedName.ToTitleCase()).ToList(),
            Publisher = book.Publisher?.Name,
            Language = book.Language,
            PublicationDate = book.PublishDate,
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

    public static Book ToDomain(this CreateBookDto bookDto)
    {
        var dataSourceParsed = Enum.TryParse<DataSource>(bookDto.DataSource, true, out var ds);

        return Book.Reconstitute(
            id: null,
            isbn10: bookDto.Isbn10,
            isbn13: bookDto.Isbn13,
            title: bookDto.Title,
            pages: bookDto.Pages,
            isRead: bookDto.IsRead,
            publishDate: bookDto.PublicationDate,
            language: bookDto.Language,
            synopsis: bookDto.Synopsis,
            imageThumbnail: bookDto.ImageThumbnail,
            imageUrl: bookDto.ImageUrl,
            msrp: bookDto.Msrp,
            binding: bookDto.Binding,
            edition: bookDto.Edition,
            seriesNumber: bookDto.NumberInSeries,
            dataSource: dataSourceParsed ? ds : DataSource.Unknown,
            publisher: bookDto.Publisher != null ? Publisher.Create(bookDto.Publisher) : null,
            series: bookDto.Series != null ? Series.Create(bookDto.Series) : null,
            dimensions: BookDimensions.Create(
                bookDto.Dimensions?.HeightCm,
                bookDto.Dimensions?.ThicknessCm,
                bookDto.Dimensions?.WidthCm,
                bookDto.Dimensions?.WeightG),
            authors: bookDto.Authors?.Select(Author.Create).ToList(),
            subjects: bookDto.Subjects?.Select(Subject.Create).ToList(),
            deweyDecimals: bookDto.DeweyDecimal?.Select(c => new DeweyDecimal { Code = c }).ToList()
        );
    }
}