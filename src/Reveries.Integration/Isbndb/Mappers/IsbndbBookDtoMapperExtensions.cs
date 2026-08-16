using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Enums;
using Reveries.Domain.Helpers;
using Reveries.Domain.Works;
using Reveries.Integration.Isbndb.DTOs.Books;

namespace Reveries.Integration.Isbndb.Mappers;

public static class IsbndbBookDtoMapperExtensions
{
    public static EditionWithWork? ToEditionWithWork(this IsbndbBookDto isbndbBookDto)
    {
        if (string.IsNullOrWhiteSpace(isbndbBookDto.Isbn13) && string.IsNullOrWhiteSpace(isbndbBookDto.Isbn10))
            return null;

        var thickness = isbndbBookDto.DimensionsStructured?.Length.ConvertDimension();
        var height = isbndbBookDto.DimensionsStructured?.Height.ConvertDimension();
        var width = isbndbBookDto.DimensionsStructured?.Width.ConvertDimension();

        var (normalizedHeight, normalizedWidth, normalizedThickness) = BookDimensionNormalizer.OrderDimensionsBySize(height, width, thickness);

        var work = Work.Create(
            title: isbndbBookDto.Title,
            authors: isbndbBookDto.Authors,
            subjects: isbndbBookDto.Subjects,
            deweyDecimals: isbndbBookDto.DeweyDecimals,
            synopsis: isbndbBookDto.Synopsis);

        var edition = Edition.Create(
            workId: work.Id,
            isbn13: isbndbBookDto.Isbn13,
            isbn10: isbndbBookDto.Isbn10,
            publisher: isbndbBookDto.Publisher,
            pages: isbndbBookDto.Pages,
            publishDate: isbndbBookDto.DatePublished,
            languageIso639: isbndbBookDto.Language,
            binding: isbndbBookDto.Binding,
            editionStatement: isbndbBookDto.Edition,
            imageThumbnail: isbndbBookDto.Image,
            imageUrl: isbndbBookDto.ImageOriginal,
            msrp: isbndbBookDto.Msrp,
            height: normalizedHeight,
            width: normalizedWidth,
            thickness: normalizedThickness,
            weight: isbndbBookDto.DimensionsStructured?.Weight.ConvertDimension(),
            dataSource: DataSource.IsbndbApi);

        return new EditionWithWork(edition, work);
    }

    private static decimal? ConvertDimension(this DimensionDto? dimension)
    {
        if (dimension is null) return null;

        var unit = dimension.Unit!.ToLowerInvariant();
        var value = dimension.Value;

        const double inchToCentimeterConversion = 2.54;
        const double poundToGramConversion = 453.59;

        var newValue = unit switch
        {
            "inches" => value * inchToCentimeterConversion,
            "pounds" => value * poundToGramConversion,
            _ => value
        };

        return (decimal?)Math.Round(newValue, 2, MidpointRounding.AwayFromZero);
    }
}