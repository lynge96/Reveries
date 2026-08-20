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

        var (primaryGenres, secondaryGenres) = googleBookDto.Categories.SplitGenres();

        var work = Work.Create(
            title: googleBookDto.Title,
            authors: googleBookDto.Authors,
            primaryGenres: primaryGenres,
            secondaryGenres: secondaryGenres,
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