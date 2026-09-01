using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
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

        var work = Work.Create(new WorkData(
            Title: isbndbBookDto.Title,
            Subtitle: null,
            Authors: isbndbBookDto.Authors,
            PrimaryGenres: null,
            SecondaryGenres: isbndbBookDto.Subjects,
            DeweyDecimals: isbndbBookDto.DeweyDecimals,
            Synopsis: isbndbBookDto.Synopsis,
            Description: null));

        var dimensions = BookDimensions.Create(
            normalizedHeight,
            normalizedWidth,
            normalizedThickness,
            isbndbBookDto.DimensionsStructured?.Weight.ConvertDimension());

        var edition = Edition.Create(new EditionData(
            WorkId: work.Id,
            Isbn13: isbndbBookDto.Isbn13,
            Isbn10: isbndbBookDto.Isbn10,
            Publisher: isbndbBookDto.Publisher,
            Pages: isbndbBookDto.Pages,
            PublishDate: isbndbBookDto.DatePublished,
            LanguageIso639: isbndbBookDto.Language,
            Format: isbndbBookDto.Binding,
            EditionStatement: isbndbBookDto.Edition,
            ImageThumbnail: isbndbBookDto.Image,
            ImageUrl: isbndbBookDto.ImageOriginal,
            SaxoUrl: null,
            Dimensions: dimensions));

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