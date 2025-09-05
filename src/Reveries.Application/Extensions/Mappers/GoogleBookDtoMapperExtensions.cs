using Reveries.Application.DTOs.GoogleBooksDtos;
using Reveries.Core.Entities;
using Reveries.Core.Enums;

namespace Reveries.Application.Extensions.Mappers;

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
            Synopsis = dto.Description,
            Isbn10 = dto.IndustryIdentifiers?
                .FirstOrDefault(i => i.Type == "ISBN_10")?.Identifier,
            Isbn13 = dto.IndustryIdentifiers?
                .FirstOrDefault(i => i.Type == "ISBN_13")?.Identifier,
            Pages = dto.PageCount,
            Subjects = dto.Categories?
                .Select(c => new Subject { Genre = c })
                .ToList() ?? [],
            LanguageIso639 = dto.Language,
            Binding = dto.PrintType
        };
    }
}