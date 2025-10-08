using Reveries.Core.Enums;
using Reveries.Core.Helpers;
using Reveries.Core.Models;
using Reveries.Integration.GoogleBooks.DTOs;

namespace Reveries.Integration.GoogleBooks.Mappers;

public static class GoogleBookDtoMapperExtensions
{
    public static Book ToBook(this GoogleVolumeInfoDto googleBookDto)
    {
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
            dimensions: BookDimensions.Create(
                heightCm: googleBookDto.Dimensions?.Height.ParseDimension(),
                widthCm: googleBookDto.Dimensions?.Width.ParseDimension(),
                thicknessCm: googleBookDto.Dimensions?.Thickness.ParseDimension(),
                weightG: null), 
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
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}