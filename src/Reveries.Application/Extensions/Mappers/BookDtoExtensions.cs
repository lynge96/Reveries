using System.Text.RegularExpressions;
using Reveries.Core.DTOs.Books;
using Reveries.Core.Models;
using System.Globalization;

namespace Reveries.Application.Extensions.Mappers;

public static class BookDtoExtensions
{
    public static Book ToBook(this BookDto bookDto)
    {
        return new Book
        {
            Isbn13 = bookDto.Isbn13,
            Isbn10 = bookDto.Isbn10,
            Title = bookDto.Title,
            Authors = bookDto.Authors?
                .Select(authorName => new Author { Name = authorName })
                .ToList() ?? new List<Author>(),
            Pages = bookDto.Pages,
            Publisher = string.IsNullOrEmpty(bookDto.Publisher) 
                ? null 
                : new Publisher { Name = bookDto.Publisher },
            LanguageIso639 = bookDto.Language,
            Language = GetLanguageName(bookDto.Language),
            PublishDate = ParsePublishDate(bookDto.DatePublished),
            Synopsis = CleanString(bookDto.Synopsis),
            ImageUrl = bookDto.ImageOriginal,
            Msrp = bookDto.Msrp,
            Binding = bookDto.Binding,
            // Edition = bookDto.Edition,
            Subjects = bookDto.Subjects?
                .Select(subjectName => new Subject { Name = subjectName })
                .ToList() ?? new List<Subject>(),
            Dimensions = bookDto.DimensionsStructured?.ToModel()
        };
    }

    private static DateTime? ParsePublishDate(string? dateString)
    {
        if (string.IsNullOrEmpty(dateString))
            return null;
            
        return DateTime.TryParse(dateString, out var date) ? date : null;
    }

    private static string CleanString(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // Først erstat HTML line breaks med mellemrum
        var noHtmlBreaks = Regex.Replace(input, "<br/?>", " ");
    
        // Fjern HTML tags
        var noHtml = Regex.Replace(noHtmlBreaks, "<.*?>", string.Empty);
    
        // Erstat alle typer af linjeskift med mellemrum
        var noLineBreaks = Regex.Replace(noHtml, @"[\r\n\t]+", " ");
    
        // Fjern mellemrum
        var singleSpaces = Regex.Replace(noLineBreaks, @"\s+", " ");
    
        // Trim mellemrum i start og slut
        return singleSpaces.Trim();
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