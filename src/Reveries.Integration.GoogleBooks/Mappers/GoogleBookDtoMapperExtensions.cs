using Reveries.Application.Common.Mappers;
using Reveries.Application.Extensions;
using Reveries.Core.Enums;
using Reveries.Core.Models;
using Reveries.Integration.GoogleBooks.DTOs;

namespace Reveries.Integration.GoogleBooks.Mappers;

public static class GoogleBookDtoMapperExtensions
{
    public static Book ToBook(this GoogleVolumeInfoDto dto)
    {
        return new Book
        {
            DataSource = DataSource.GoogleBooksApi,
            Title = dto.Title,
            Authors = dto.Authors?
                .Select(authorName =>
                {
                    var (firstName, lastName, normalizedName) = AuthorNameNormalizer.NormalizeAuthorName(authorName);
                    return new Author
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        NormalizedName = normalizedName
                    };
                })
                .ToList() ?? [],
            Edition = dto.Subtitle,
            Publisher = string.IsNullOrEmpty(dto.Publisher) 
                ? null 
                : new Publisher { Name = PublisherNormalizer.NormalizePublisher(dto.Publisher) },
            PublishDate = dto.PublishedDate.ParsePublishDate(),
            Synopsis = dto.Description.CleanHtml(),
            Isbn10 = dto.IndustryIdentifiers?
                .FirstOrDefault(i => i.Type == "ISBN_10")?.Identifier,
            Isbn13 = dto.IndustryIdentifiers?
                .FirstOrDefault(i => i.Type == "ISBN_13")?.Identifier,
            Pages = dto.PageCount,
            Subjects = dto.Categories?.ExtractUniqueSubjects(),
            Language = dto.Language.GetLanguageName(),
            LanguageIso639 = dto.Language,
            Binding = dto.PrintType.ToTitleCase(),
            ImageThumbnail = dto.ImageLinks?.Thumbnail,
            Dimensions = new BookDimensions
            {
                HeightCm = dto.Dimensions?.Height.ParseDimension(),
                WidthCm = dto.Dimensions?.Width.ParseDimension(),
                ThicknessCm = dto.Dimensions?.Thickness.ParseDimension()
            }
        };
    }

    private static List<Subject> ExtractUniqueSubjects(this IEnumerable<string>? categories)
    {
        if (categories == null)
            return new List<Subject>();

        return categories
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .SelectMany(c => c.Split('/', StringSplitOptions.TrimEntries))
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(c => new Subject { Genre = c })
            .ToList();
    }
    
}