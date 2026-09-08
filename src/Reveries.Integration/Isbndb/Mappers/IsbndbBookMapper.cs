using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Helpers;
using Reveries.Integration.Isbndb.DTOs.Books;

namespace Reveries.Integration.Isbndb.Mappers;

public static class IsbndbBookMapper
{
    private const double InchToCentimeter = 2.54;
    private const double PoundToGram = 453.59;

    public static BookCandidate? ToBookCandidate(this IsbndbBookDto book)
    {
        if (string.IsNullOrWhiteSpace(book.Isbn13) && string.IsNullOrWhiteSpace(book.Isbn10))
            return null;

        return BookCandidate.Create(new BookCandidateData(
            Isbn13: book.Isbn13,
            Isbn10: book.Isbn10,
            Title: book.Title ?? string.Empty,
            Subtitle: null,
            Authors: book.Authors,
            Publisher: book.Publisher,
            PrimaryGenres: null,
            SecondaryGenres: book.Subjects,
            DeweyDecimals: book.DeweyDecimals,
            Synopsis: book.Synopsis,
            Description: null,
            Pages: book.Pages,
            PublishDate: book.DatePublished,
            LanguageIso639: book.Language,
            Format: book.Binding,
            EditionStatement: book.Edition,
            ImageThumbnail: book.Image,
            ImageUrl: book.ImageOriginal,
            Dimensions: ToDimensions(book.DimensionsStructured)));
    }

    private static BookDimensions? ToDimensions(IsbndbDimensionsDto? dimensions)
    {
        if (dimensions is null)
            return null;

        var height = ToNormalizedValue(dimensions.Height);
        var width = ToNormalizedValue(dimensions.Width);
        var thickness = ToNormalizedValue(dimensions.Length);

        var (orderedHeight, orderedWidth, orderedThickness) =
            BookDimensionNormalizer.OrderDimensionsBySize(height, width, thickness);

        return BookDimensions.Create(
            orderedHeight,
            orderedWidth,
            orderedThickness,
            ToNormalizedValue(dimensions.Weight));
    }

    private static decimal? ToNormalizedValue(IsbndbDimensionDto? dimension)
    {
        if (dimension?.Value is not { } value)
            return null;

        var converted = dimension.Unit?.ToLowerInvariant() switch
        {
            "inches" => value * InchToCentimeter,
            "pounds" => value * PoundToGram,
            _ => value
        };

        return (decimal)Math.Round(converted, 2, MidpointRounding.AwayFromZero);
    }
}