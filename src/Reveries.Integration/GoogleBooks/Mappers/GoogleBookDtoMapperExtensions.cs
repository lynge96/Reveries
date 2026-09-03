using System.Globalization;
using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Helpers;
using Reveries.Integration.GoogleBooks.DTOs;

namespace Reveries.Integration.GoogleBooks.Mappers;

public static class GoogleBookDtoMapperExtensions
{
    public static BookCandidate? ToBookCandidate(this GoogleVolumeInfoDto googleBookDto)
    {
        var isbn13 = googleBookDto.IndustryIdentifiers?.FirstOrDefault(i => i.Type == "ISBN_13")?.Identifier;
        var isbn10 = googleBookDto.IndustryIdentifiers?.FirstOrDefault(i => i.Type == "ISBN_10")?.Identifier;

        if (string.IsNullOrWhiteSpace(isbn13) && string.IsNullOrWhiteSpace(isbn10))
            return null;

        var thickness = googleBookDto.Dimensions?.Thickness.ParseDimension();
        var height = googleBookDto.Dimensions?.Height.ParseDimension();
        var width = googleBookDto.Dimensions?.Width.ParseDimension();

        var (normalizedHeight, normalizedWidth, normalizedThickness) = BookDimensionNormalizer.OrderDimensionsBySize(height, width, thickness);

        var dimensions = BookDimensions.Create(normalizedHeight, normalizedWidth, normalizedThickness, null);

        var (primaryGenres, secondaryGenres) = googleBookDto.Categories.SplitGenres();

        return BookCandidate.Create(new BookCandidateData(
            Isbn13: isbn13,
            Isbn10: isbn10,
            Title: googleBookDto.Title,
            Subtitle: googleBookDto.Subtitle,
            Authors: googleBookDto.Authors,
            Publisher: googleBookDto.Publisher,
            PrimaryGenres: primaryGenres,
            SecondaryGenres: secondaryGenres,
            DeweyDecimals: null,
            Synopsis: googleBookDto.Description,
            Description: googleBookDto.Description,
            Pages: googleBookDto.PageCount,
            PublishDate: googleBookDto.PublishedDate,
            LanguageIso639: googleBookDto.Language,
            Format: googleBookDto.PrintType,
            EditionStatement: null,
            ImageThumbnail: googleBookDto.ImageLinks?.Thumbnail,
            ImageUrl: googleBookDto.ImageLinks?.Thumbnail,
            Dimensions: dimensions));
    }

    private static (List<string> Primary, List<string> Secondary) SplitGenres(this IEnumerable<string>? categories)
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