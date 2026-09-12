using System.Globalization;
using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Helpers;
using Reveries.Integration.GoogleBooks.Dtos;

namespace Reveries.Integration.GoogleBooks.Mappers;

public static class GoogleBooksMapper
{
    public static BookCandidate? ToBookCandidate(this GoogleVolumeInfoDto volumeInfo)
    {
        var isbn13 = volumeInfo.IndustryIdentifiers?.FirstOrDefault(i => i.Type == "ISBN_13")?.Identifier;
        var isbn10 = volumeInfo.IndustryIdentifiers?.FirstOrDefault(i => i.Type == "ISBN_10")?.Identifier;

        if (string.IsNullOrWhiteSpace(isbn13) && string.IsNullOrWhiteSpace(isbn10))
            return null;

        var height = volumeInfo.Dimensions?.Height.ParseDimension();
        var width = volumeInfo.Dimensions?.Width.ParseDimension();
        var thickness = volumeInfo.Dimensions?.Thickness.ParseDimension();

        var (orderedHeight, orderedWidth, orderedThickness) =
            BookDimensionNormalizer.OrderDimensionsBySize(height, width, thickness);

        var dimensions = BookDimensions.Create(orderedHeight, orderedWidth, orderedThickness, null);

        var (primaryGenres, secondaryGenres) = SplitGenres(volumeInfo.Categories);

        return BookCandidate.Create(new BookCandidateData(
            Isbn13: isbn13,
            Isbn10: isbn10,
            Title: volumeInfo.Title ?? string.Empty,
            Subtitle: volumeInfo.Subtitle,
            Authors: volumeInfo.Authors,
            Publisher: volumeInfo.Publisher,
            PrimaryGenres: primaryGenres,
            SecondaryGenres: secondaryGenres,
            DeweyDecimals: null,
            Synopsis: volumeInfo.Description,
            Description: volumeInfo.Description,
            Pages: volumeInfo.PageCount,
            PublishDate: volumeInfo.PublishedDate,
            LanguageIso639: volumeInfo.Language,
            Format: volumeInfo.PrintType,
            EditionStatement: null,
            ImageThumbnail: volumeInfo.ImageLinks?.SmallThumbnail,
            ImageUrl: volumeInfo.ImageLinks?.Thumbnail,
            Dimensions: dimensions));
    }

    private static (List<string> Primary, List<string> Secondary) SplitGenres(IEnumerable<string>? categories)
    {
        var primary = new List<string>();
        var secondary = new List<string>();

        foreach (var category in categories ?? [])
        {
            if (string.IsNullOrWhiteSpace(category))
                continue;

            var segments = category.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0)
                continue;

            primary.Add(segments[0]);
            secondary.AddRange(segments.Skip(1));
        }

        return (primary, secondary);
    }

    private static decimal? ParseDimension(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var numericPart = value.Replace("cm", "", StringComparison.OrdinalIgnoreCase).Trim();

        return decimal.TryParse(numericPart, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;
    }
}