using Reveries.Core.Enums;
using Reveries.Core.Exceptions;
using Reveries.Core.Helpers;
using Reveries.Core.Validation;

namespace Reveries.Core.Models;

public class Book : BaseEntity
{
    private readonly List<Author> _authors = [];
    private readonly List<Subject> _subjects = [];
    private readonly List<DeweyDecimal> _deweyDecimals = [];
    
    public int? Id { get; init; }
    public string? Isbn13 { get; init; }
    public string? Isbn10 { get; init; }
    public required string Title { get; init; }
    public IReadOnlyList<Author> Authors => _authors;
    public int? Pages { get; private set; }
    public bool IsRead { get; private set; }
    public Publisher? Publisher { get; private set; }
    public string? Language { get; init; }
    public string? PublishDate { get; init; }
    public string? Synopsis { get; init; }
    public string? ImageThumbnail { get; init; }
    public string? ImageUrl { get; init; }
    public decimal? Msrp { get; init; }
    public string? Binding { get; init; }
    public string? Edition { get; init; }
    public IReadOnlyList<DeweyDecimal> DeweyDecimals => _deweyDecimals;
    public IReadOnlyList<Subject> Subjects => _subjects;
    public int? SeriesNumber { get; private set; }
    public Series? Series { get; private set; }
    public BookDimensions? Dimensions { get; init; }
    public DataSource DataSource { get; private set; }

    private Book() { }
    
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
    
    /// <summary>
    /// Factory method for creating a new <see cref="Book"/> from external or user-provided data.
    ///
    /// This method represents the "entry point" into the domain. It performs validation,
    /// normalization, and transformation of raw input into a consistent domain model.
    /// Examples include normalizing ISBNs, resolving language codes, standardizing bindings,
    /// and creating related value objects such as authors, subjects, and dimensions.
    ///
    /// Use this method when:
    /// - Importing books from external APIs
    /// - Creating new books from user input
    /// - You want domain invariants to be enforced
    ///
    /// This method will throw if required invariants are violated (e.g. missing title).
    /// </summary>
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
            throw new MissingTitleException(title);
        
        var book = new Book
        {
            Isbn10 = IsbnValidator.NormalizeIsbn(isbn10 ?? null),
            Isbn13 = IsbnValidator.NormalizeIsbn(isbn13 ?? null),
            Title = title,
            Pages = pages,
            IsRead = false,
            PublishDate = publishDate,
            Publisher = Publisher.Create(publisher),
            Language = languageIso639.GetLanguageName(),
            Synopsis = synopsis,
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
            DataSource = dataSource,
        };

        book.SetPages(pages);
        
        foreach (var authorName in authors ?? [])
        {
            var author = Author.Create(authorName);
            book.AddAuthor(author);
        }
        
        foreach (var subject in subjects ?? [])
        {
            var genre = Subject.Create(subject);
            book.AddSubject(genre);
        }
        
        foreach (var code in deweyDecimals ?? [])
        {
            var dewey = DeweyDecimal.Create(code);
            book.AddDeweyDecimal(dewey);
        }
        
        return book;
    }
    
    /// <summary>
    /// Recreates a <see cref="Book"/> from already persisted data.
    ///
    /// This method is used when loading a book from storage (database, merges, cache, etc.).
    /// It assumes that all domain invariants were previously validated and therefore
    /// does NOT perform normalization, validation, or transformation of values.
    ///
    /// Use this method when:
    /// - Hydrating entities from a database
    /// - Rebuilding an aggregate from persistence
    /// - You need full control over the internal state
    ///
    /// This method should not throw due to domain validation failures,
    /// as it trusts the persisted data.
    /// </summary>
    public static Book Reconstitute(
        int? id,
        string? isbn13,
        string? isbn10,
        string title,
        int? pages,
        bool isRead,
        string? publishDate,
        string? language,
        string? synopsis,
        string? imageThumbnail,
        string? imageUrl,
        decimal? msrp,
        string? binding,
        string? edition,
        int? seriesNumber,
        DataSource dataSource,
        DateTimeOffset? dateCreated = null,
        Publisher? publisher = null,
        Series? series = null,
        BookDimensions? dimensions = null,
        IEnumerable<Author>? authors = null,
        IEnumerable<Subject>? subjects = null,
        IEnumerable<DeweyDecimal>? deweyDecimals = null
    )
    {
        var book = new Book
        {
            Id = id,
            Isbn13 = isbn13,
            Isbn10 = isbn10,
            Title = title,
            Pages = pages,
            IsRead = isRead,
            PublishDate = publishDate,
            Publisher = publisher,
            Language = language,
            Synopsis = synopsis,
            ImageThumbnail = imageThumbnail,
            ImageUrl = imageUrl,
            Msrp = msrp,
            Binding = binding,
            Edition = edition,
            SeriesNumber = seriesNumber,
            Series = series,
            Dimensions = dimensions,
            DataSource = dataSource,
            DateCreated = dateCreated
        };

        if (authors != null)
        {
            book._authors.AddRange(authors);
        }

        if (subjects != null)
        {
            book._subjects.AddRange(subjects);
        }

        if (deweyDecimals != null)
        {
            book._deweyDecimals.AddRange(deweyDecimals);
        }

        return book;
    }
    
    public void UpdateDataSource(DataSource newDataSource)
    {
        if (DataSource == newDataSource) return;
        
        DataSource = newDataSource;
    }
    
    public void MarkAsRead()
    {
        if (IsRead) return;
        IsRead = true;
    }

    public void MarkAsUnread()
    {
        if (!IsRead) return;
        IsRead = false;
    }

    private void SetPages(int? pages)
    {
        if (pages is null)
            return;

        if (pages <= 0)
            throw new InvalidPageCountException(pages);

        Pages = pages;
    }
    
    public void SetPublisher(Publisher publisher)
    {
        Publisher = publisher;
    }
    
    public void SetSeries(Series series, int? numberInSeries = null)
    {
        if (numberInSeries <= 0)
            throw new InvalidSeriesNumberException(numberInSeries);
            
        Series = series;
        SeriesNumber = numberInSeries;
    }

    public void AddAuthor(Author? author)
    {
        if (_authors.Any(a => a.NormalizedName == author?.NormalizedName) || author is null) return;
        _authors.Add(author);
    }
    
    public void AddSubject(Subject? subject)
    {
        if (_subjects.Any(s => s.Genre == subject?.Genre) || subject is null) return;
        _subjects.Add(subject);
    }

    public void AddDeweyDecimal(DeweyDecimal? deweyDecimal)
    {
        if (_deweyDecimals.Any(dd => dd.Code == deweyDecimal?.Code) || deweyDecimal is null) return;
        _deweyDecimals.Add(deweyDecimal);
    }
}

