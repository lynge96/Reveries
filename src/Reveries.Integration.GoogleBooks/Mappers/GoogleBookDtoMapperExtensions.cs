using System.Globalization;
using Reveries.Core.Enums;
using Reveries.Core.Helpers;
using Reveries.Core.Models;
using Reveries.Integration.GoogleBooks.DTOs;

namespace Reveries.Integration.GoogleBooks.Mappers;

public static class GoogleBookDtoMapperExtensions
{
    public static Book ToBook(this GoogleVolumeInfoDto googleBookDto)
    {
        var thickness = googleBookDto.Dimensions?.Thickness.ParseDimension();
        var height = googleBookDto.Dimensions?.Height.ParseDimension();
        var width = googleBookDto.Dimensions?.Width.ParseDimension();
        
        var (normalizedHeight, normalizedWidth, normalizedThickness) = DimensionNormalizer.NormalizeDimensions(height, width, thickness);

        return Book.Create(
            isbn13: googleBookDto.IndustryIdentifiers?
                .FirstOrDefault(i => i.Type == "ISBN_13")?.Identifier,
            isbn10: googleBookDto.IndustryIdentifiers?
                .FirstOrDefault(i => i.Type == "ISBN_10")?.Identifier,
            title: googleBookDto.Title,
            authors: googleBookDto.Authors,
            pages: googleBookDto.PageCount,
            publishDate: googleBookDto.PublishedDate,
            publisher: googleBookDto.Publisher,
            languageIso639: googleBookDto.Language,
            synopsis: googleBookDto.Description,
            imageThumbnail: googleBookDto.ImageLinks?.Thumbnail,
            imageUrl: null,
            msrp: null,
            binding: googleBookDto.PrintType,
            edition: googleBookDto.Subtitle,
            weight: null,
            thickness: normalizedThickness,
            height: normalizedHeight,
            width: normalizedWidth,
            subjects: googleBookDto.Categories.ExtractUniqueSubjects(),
            deweyDecimals: null,
            dataSource: DataSource.GoogleBooksApi
        );
    }

    private static List<string> ExtractUniqueSubjects(this IEnumerable<string>? categories)
    {
        if (categories == null)
            return new List<string>();

        return categories
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .SelectMany(c => c.Split('/', StringSplitOptions.TrimEntries))
            .Select(c => c.ToTitleCase())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
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