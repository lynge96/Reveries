using Reveries.Core.Enums;
using Reveries.Core.Helpers;
using Reveries.Core.Validation;

namespace Reveries.Core.Models;

public class Book : BaseEntity
{
    public int? Id { get; set; }
    public string? Isbn13 { get; init; }
    public string? Isbn10 { get; init; }
    public required string Title { get; init; }
    public ICollection<Author> Authors { get; set; } = new List<Author>();
    public int? Pages { get; init; }
    public bool IsRead { get; set; }
    public Publisher? Publisher { get; set; }
    public string? Language { get; init; }
    public string? PublishDate { get; init; }
    public string? Synopsis { get; init; }
    public string? ImageThumbnail { get; init; }
    public string? ImageUrl { get; init; }
    public decimal? Msrp { get; init; }
    public string? Binding { get; init; }
    public string? Edition { get; init; }
    public ICollection<DeweyDecimal>? DeweyDecimals { get; set; }
    public ICollection<Subject>? Subjects { get; set; }
    public int? SeriesNumber { get; set; }
    public Series? Series { get; set; }
    public BookDimensions? Dimensions { get; set; }
    public required DataSource DataSource { get; set; }

    public override string ToString()
    {
        var title = string.IsNullOrEmpty(Title) ? "Unknown Title" : Title;
        var authors = Authors
            .Select(a => a.ToString())
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .DefaultIfEmpty("Unknown Author");
    
        return $"{title} by {string.Join(", ", authors)}";
    }
    
    public string GetAuthorNames()
    {
        return string.Join(", ", Authors.Select(a => a.ToString()));
    }

    public static Book Create(
        string? isbn13,
        string? isbn10,
        string title,
        IEnumerable<string>? authors,
        int? pages,
        string? publishDate,
        string? publisher,
        string? languageIso639,
        string? synopsis,
        string? imageThumbnail,
        string? imageUrl,
        decimal? msrp,
        string? binding,
        string? edition,
        decimal? weight,
        decimal? thickness,
        decimal? height,
        decimal? width,
        IEnumerable<string>? subjects,
        IEnumerable<string>? deweyDecimals,
        DataSource dataSource
    )
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Book title cannot be empty");

        var (cleanedTitle, seriesName, numberInSeries) = BookExtensions.ParseSeriesInfo(title);
        
        var book = new Book
        {
            Isbn10 = IsbnValidator.NormalizeIsbn(isbn10 ?? string.Empty),
            Isbn13 = IsbnValidator.NormalizeIsbn(isbn13 ?? string.Empty),
            Title = cleanedTitle,
            Authors = (authors ?? [])
                .Select(Author.Create)
                .ToList(),
            Pages = pages,
            IsRead = false,
            PublishDate = publishDate,
            Publisher = Publisher.Create(publisher),
            Language = languageIso639.GetLanguageName(),
            Synopsis = synopsis.CleanHtml(),
            ImageThumbnail = imageThumbnail,
            ImageUrl = imageUrl,
            Msrp = msrp,
            Binding = binding?.GetStandardBinding(),
            Edition = edition,
            Dimensions = BookDimensions.Create(
                height, 
                width, 
                thickness, 
                weight),
            Subjects = (subjects ?? [])
                .Select(Subject.Create)
                .ToList(),
            DeweyDecimals = deweyDecimals.FormatDeweyDecimals(),
            Series = seriesName != null ? Series.Create(seriesName) : null,
            SeriesNumber = numberInSeries,
            DataSource = dataSource,
        };

        return book;
    }
    
    public Book UpdateDataSource(DataSource newDataSource)
    {
        DataSource = newDataSource;
        return this;
    }
}

