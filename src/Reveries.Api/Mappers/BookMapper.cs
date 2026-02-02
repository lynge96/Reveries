using Reveries.Contracts.DTOs;
using Reveries.Core.Enums;
using Reveries.Core.Helpers;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Api.Mappers;

public static class BookMapper
{
    public static BookDto ToDto(this Book book)
    {
        return new BookDto
        {
            Id = book.Id.Value,
            Isbn10 = book.Isbn10?.Value,
            Isbn13 = book.Isbn13?.Value,
            Title = book.Title,
            Authors = book.Authors.Select(a => a.NormalizedName.ToTitleCase()).ToList(),
            Publisher = book.Publisher?.Name,
            Language = book.Language,
            PublicationDate = book.PublishDate,
            Pages = book.Pages,
            Synopsis = book.Synopsis,
            ImageUrl = book.CoverImageUrl,
            ImageThumbnail = book.ImageThumbnailUrl,
            Msrp = book.Msrp,
            Binding = book.Binding,
            Edition = book.Edition,
            Subjects = book.Genres?.Select(g => g.Value).ToList() ?? [],
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
            DeweyDecimal = book.DeweyDecimals?.Select(d => d.Code).ToList() ?? [],
            DataSource = book.DataSource.ToString()
        };
    }

    public static Book ToDomain(this CreateBookDto bookDto)
    {
        var dataSourceParsed = Enum.TryParse<DataSource>(bookDto.DataSource, true, out var ds);

        return Book.Create(
            isbn10: bookDto.Isbn10,
            isbn13: bookDto.Isbn13,
            title: bookDto.Title,
            pages: bookDto.Pages,
            publishDate: bookDto.PublicationDate,
            languageIso639: bookDto.Language,
            synopsis: bookDto.Synopsis,
            imageThumbnail: bookDto.ImageThumbnail,
            imageUrl: bookDto.ImageUrl,
            msrp: bookDto.Msrp,
            binding: bookDto.Binding,
            edition: bookDto.Edition,
            dataSource: dataSourceParsed ? ds : DataSource.Unknown,
            publisher: bookDto.Publisher,
            weight: bookDto.Dimensions?.WeightG,
            height: bookDto.Dimensions?.HeightCm,
            width: bookDto.Dimensions?.WidthCm,
            thickness: bookDto.Dimensions?.ThicknessCm,
            authors: bookDto.Authors,
            subjects: bookDto.Subjects,
            deweyDecimals: bookDto.DeweyDecimal
        );
    }
}