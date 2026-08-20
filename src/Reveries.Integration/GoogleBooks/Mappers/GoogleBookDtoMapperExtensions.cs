using System.Globalization;
using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Enums;
using Reveries.Domain.Helpers;
using Reveries.Domain.Works;
using Reveries.Integration.GoogleBooks.DTOs;

namespace Reveries.Integration.GoogleBooks.Mappers;

public static class GoogleBookDtoMapperExtensions
{
    public static EditionWithWork? ToEditionWithWork(this GoogleVolumeInfoDto googleBookDto)
    {
        var isbn13 = googleBookDto.IndustryIdentifiers?.FirstOrDefault(i => i.Type == "ISBN_13")?.Identifier;
        var isbn10 = googleBookDto.IndustryIdentifiers?.FirstOrDefault(i => i.Type == "ISBN_10")?.Identifier;

        if (string.IsNullOrWhiteSpace(isbn13) && string.IsNullOrWhiteSpace(isbn10))
            return null;

        var thickness = googleBookDto.Dimensions?.Thickness.ParseDimension();
        var height = googleBookDto.Dimensions?.Height.ParseDimension();
        var width = googleBookDto.Dimensions?.Width.ParseDimension();

        var (normalizedHeight, normalizedWidth, normalizedThickness) = BookDimensionNormalizer.OrderDimensionsBySize(height, width, thickness);

        var work = Work.Create(
            title: googleBookDto.Title,
            authors: googleBookDto.Authors,
            genres: googleBookDto.Categories.ExtractUniqueSubjects(),
            deweyDecimals: null,
            synopsis: googleBookDto.Description);

        var edition = Edition.Create(
            workId: work.Id,
            isbn13: isbn13,
            isbn10: isbn10,
            publisher: googleBookDto.Publisher,
            pages: googleBookDto.PageCount,
            publishDate: googleBookDto.PublishedDate,
            languageIso639: googleBookDto.Language,
            binding: googleBookDto.PrintType,
            editionStatement: googleBookDto.Subtitle,
            imageThumbnail: googleBookDto.ImageLinks?.Thumbnail,
            imageUrl: googleBookDto.ImageLinks?.Thumbnail,
            msrp: null,
            height: normalizedHeight,
            width: normalizedWidth,
            thickness: normalizedThickness,
            weight: null,
            dataSource: DataSource.GoogleBooksApi);

        return new EditionWithWork(edition, work);
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