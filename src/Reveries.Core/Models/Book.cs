using System.Globalization;
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
    public ICollection<Author> Authors { get; init; } = new List<Author>();
    public int? Pages { get; init; }
    public bool IsRead { get; set; }
    public Publisher? Publisher { get; init; }
    public string? LanguageIso639 { get; init; }
    public string? Language { get; init; }
    public DateTime? PublishDate { get; init; }
    public string PublishDateFormatted => PublishDate?
        .ToString("dd-MM-yyyy", CultureInfo.InvariantCulture) ?? "Unknown Date";
    public string? Synopsis { get; init; }
    public string? ImageThumbnail { get; init; }
    public string? ImageUrl { get; init; }
    public decimal? Msrp { get; init; }
    public string? Binding { get; init; }
    public string? Edition { get; init; }
    public ICollection<DeweyDecimal>? DeweyDecimals { get; init; }
    public ICollection<Subject>? Subjects { get; init; }
    public int? SeriesNumber { get; private set; }
    public Series? Series { get; private set; }
    public BookDimensions? Dimensions { get; init; }
    public DataSource DataSource { get; private set; }

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
        BookDimensions? dimensions,
        IEnumerable<string>? subjects,
        IEnumerable<string>? deweyDecimals,
        DataSource dataSource
        )
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Book title cannot be empty");
        IsbnValidator.ValidateOrThrow(isbn13, isbn10);

        var (cleanedTitle, seriesName, numberInSeries) = BookExtensions.ParseSeriesInfo(title);
        
        var book = new Book
        {
            Isbn10 = isbn10,
            Isbn13 = isbn13,
            Title = cleanedTitle,
            Authors = (authors ?? [])
                .Select(Author.Create)
                .ToList(),
            Pages = pages,
            IsRead = false,
            PublishDate = publishDate.ParsePublishDate(),
            Publisher = Publisher.Create(publisher),
            LanguageIso639 = languageIso639,
            Language = languageIso639.GetLanguageName(),
            Synopsis = synopsis.CleanHtml(),
            ImageThumbnail = imageThumbnail,
            ImageUrl = imageUrl,
            Msrp = msrp,
            Binding = binding?.NormalizeBinding(),
            Edition = edition,
            Dimensions = BookDimensions.Create(
                dimensions?.HeightCm, 
                dimensions?.WidthCm, 
                dimensions?.ThicknessCm, 
                dimensions?.WeightG),
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

    public void SetSeriesNumber(int? seriesNumber)
    {
        SeriesNumber = seriesNumber;
    }

    public void UpdateSeries(Series series)
    {
        Series = series;
    }
    
    public Book UpdateDataSource(DataSource newDataSource)
    {
        DataSource = newDataSource;
        return this;
    }
}

