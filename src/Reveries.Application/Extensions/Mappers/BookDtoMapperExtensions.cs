using System.Text.RegularExpressions;
using System.Globalization;
using Reveries.Application.DTOs.Books;
using Reveries.Core.Entities;
using Reveries.Core.Enums;

namespace Reveries.Application.Extensions.Mappers;

public static class BookDtoMapperExtensions
{
    public static Book ToBook(this BookDto bookDto)
    {
        return new Book
        {
            Isbn13 = bookDto.Isbn13,
            Isbn10 = bookDto.Isbn10,
            Title = bookDto.Title,
            Authors = bookDto.Authors?
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
                .ToList() ?? new List<Author>(),
            Pages = bookDto.Pages,
            Publisher = string.IsNullOrEmpty(bookDto.Publisher) 
                ? null 
                : new Publisher { Name = bookDto.Publisher },
            LanguageIso639 = bookDto.Language,
            Language = GetLanguageName(bookDto.Language),
            PublishDate = ParsePublishDate(bookDto.DatePublished),
            Synopsis = bookDto.Synopsis.CleanHtml(),
            ImageThumbnail = bookDto.Image,
            ImageUrl = bookDto.ImageOriginal,
            Msrp = bookDto.Msrp,
            Binding = bookDto.Binding,
            Edition = bookDto.Edition,
            Subjects = bookDto.Subjects?
                .Select(subjectName => new Subject { Genre = subjectName })
                .ToList() ?? new List<Subject>(),
            Dimensions = bookDto.DimensionsStructured?.ToModel(),
            DataSource = DataSource.IsbndbApi,
            DeweyDecimals = bookDto.DeweyDecimals?
                .Select(code => new DeweyDecimal { Code = code })
                .ToList() ?? new List<DeweyDecimal>()
        };
    }

    private static DateTime? ParsePublishDate(string? dateString)
    {
        if (string.IsNullOrEmpty(dateString))
            return null;
            
        return DateTime.TryParse(dateString, out var date) ? date : null;
    }
    
    private static string GetLanguageName(string? languageIso639)
    {
        if (string.IsNullOrEmpty(languageIso639))
            return "Unknown";

        try
        {
            var culture = CultureInfo.GetCultureInfo(languageIso639);
            return culture.EnglishName.Split(' ')[0];
        }
        catch (CultureNotFoundException)
        {
            try
            {
                var cultureWithRegion = CultureInfo.GetCultureInfo($"{languageIso639}-{languageIso639.ToUpper()}");
                return cultureWithRegion.EnglishName.Split(' ')[0];
            }
            catch (CultureNotFoundException)
            {
                return "Unknown";
            }
        }
    }



}