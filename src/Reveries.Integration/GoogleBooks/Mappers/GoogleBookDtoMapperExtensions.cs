using System.Globalization;
using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
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

        var work = Work.Create(new WorkData(
            Title: googleBookDto.Title,
            Authors: googleBookDto.Authors,
            PrimaryGenres: primaryGenres,
            SecondaryGenres: secondaryGenres,
            DeweyDecimals: null,
            Synopsis: googleBookDto.Description,
            Description: googleBookDto.Description));

        var dimensions = BookDimensions.Create(normalizedHeight, normalizedWidth, normalizedThickness, null);

        var edition = Edition.Create(new EditionData(
            WorkId: work.Id,
            Isbn13: isbn13,
            Isbn10: isbn10,
            Publisher: googleBookDto.Publisher,
            Pages: googleBookDto.PageCount,
            PublishDate: googleBookDto.PublishedDate,
            LanguageIso639: googleBookDto.Language,
            Format: googleBookDto.PrintType,
            EditionStatement: googleBookDto.Subtitle,
            ImageThumbnail: googleBookDto.ImageLinks?.Thumbnail,
            ImageUrl: googleBookDto.ImageLinks?.Thumbnail,
            SaxoUrl: null,
            Dimensions: dimensions));

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