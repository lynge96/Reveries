using System.Text.RegularExpressions;
using Reveries.Application.DTOs.IsbndbDtos.Books;
using Reveries.Core.Entities;
using Reveries.Core.Enums;

namespace Reveries.Application.Extensions.Mappers;

public static class IsbndbBookDtoMapperExtensions
{
    public static Book ToBook(this IsbndbBookDto isbndbBookDto)
    {
        var (cleanedTitle, seriesName, seriesNumber) = ParseSeriesInfo(isbndbBookDto.Title);
        
        return new Book
        {
            Isbn13 = isbndbBookDto.Isbn13,
            Isbn10 = isbndbBookDto.Isbn10,
            Title = cleanedTitle,
            Authors = isbndbBookDto.Authors?
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
            Pages = isbndbBookDto.Pages,
            Publisher = string.IsNullOrEmpty(isbndbBookDto.Publisher) 
                ? null 
                : new Publisher { Name = PublisherNormalizer.NormalizePublisher(isbndbBookDto.Publisher) },
            LanguageIso639 = isbndbBookDto.Language,
            Language = isbndbBookDto.Language.GetLanguageName(),
            PublishDate = isbndbBookDto.DatePublished.ParsePublishDate(),
            Synopsis = isbndbBookDto.Synopsis.CleanHtml(),
            ImageThumbnail = isbndbBookDto.Image,
            ImageUrl = isbndbBookDto.ImageOriginal,
            Msrp = isbndbBookDto.Msrp,
            Binding = BindingNormalizer.Normalize(isbndbBookDto.Binding),
            Edition = isbndbBookDto.Edition,
            Subjects = isbndbBookDto.Subjects?
                .Select(subjectName => new Subject { Genre = subjectName })
                .ToList() ?? new List<Subject>(),
            Dimensions = isbndbBookDto.DimensionsStructured?.ToModel(),
            DataSource = DataSource.IsbndbApi,
            DeweyDecimals = isbndbBookDto.DeweyDecimals?
                .Select(code => new DeweyDecimal { Code = code })
                .ToList() ?? new List<DeweyDecimal>(),
            Series = seriesName != null ? new Series { Name = seriesName } : null,
            SeriesNumber = seriesNumber,
        };
    }
    
    private static (string cleanedTitle, string? seriesName, int? seriesNumber) ParseSeriesInfo(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return (title, null, null);

        var parentheses = Regex.Matches(title, @"\(([^)]+)\)");
        int? seriesNumber = null;
        string? seriesName = null;

        // Regex til at fange tal, inkl. "Book 1", "Vol. 2", "Volume 3"
        var numberPattern = new Regex(@"\b(?:Book|Vol(?:ume)?)?\s*(\d+)\b", RegexOptions.IgnoreCase);

        // Vi går baglæns igennem parenteser
        for (int i = parentheses.Count - 1; i >= 0; i--)
        {
            var content = parentheses[i].Groups[1].Value.Trim();

            // Format: "Series Name, Book 1" eller "Series Name, 2"
            var parts = content.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 2)
            {
                var numberMatch = numberPattern.Match(parts[1]);
                if (numberMatch.Success && int.TryParse(numberMatch.Groups[1].Value, out var numberFromParts))
                {
                    seriesName = parts[0];
                    seriesNumber = numberFromParts;
                    // Fjern denne parentes fra titlen
                    title = title.Replace($"({content})", "").Trim();
                    continue;
                }
            }

            // Hvis hele parentesen bare er "Book 1" eller "2"
            var soloMatch = numberPattern.Match(content);
            if (soloMatch.Success && int.TryParse(soloMatch.Groups[1].Value, out var soloNumber))
            {
                seriesNumber = soloNumber;
                // Fjern denne parentes fra titlen
                title = title.Replace($"({content})", "").Trim();
                continue;
            }

            // Ellers tolker vi det som serienavn
            if (!string.IsNullOrEmpty(content) && seriesName == null)
            {
                seriesName = content;
                title = title.Replace($"({content})", "").Trim();
            }
        }

        // Rens ekstra mellemrum og hængende kolon
        title = Regex.Replace(title, @"\s{2,}", " ").Trim();
        title = Regex.Replace(title, @"[:\-\.,]\s*$", "").Trim();

        return (title, seriesName, seriesNumber);
    }
    
}