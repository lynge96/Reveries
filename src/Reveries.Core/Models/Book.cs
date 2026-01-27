using Reveries.Core.Enums;
using Reveries.Core.Helpers;
using Reveries.Core.Validation;

namespace Reveries.Core.Models;

public class Book : BaseEntity
{
    private readonly List<Author> _authors = [];
    private readonly List<Subject> _subjects = [];
    private readonly List<DeweyDecimal> _deweyDecimals = [];
    
    private Book() { }
    
    public int? Id { get; set; }
    public string? Isbn13 { get; init; }
    public string? Isbn10 { get; init; }
    public required string Title { get; init; }
    public IReadOnlyList<Author> Authors => _authors;
    public int? Pages { get; init; }
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
    public int? SeriesNumber { get; set; }
    public Series? Series { get; set; }
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
            throw new ArgumentException("Book title cannot be empty");
        
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

        foreach (var authorName in authors ?? [])
        {
            book._authors.Add(
                !string.IsNullOrWhiteSpace(authorName)
                    ? Author.Create(authorName)
                    : Author.Unknown()
            );
        }
        
        foreach (var subject in subjects ?? [])
        {
            book._subjects.Add(Subject.Create(subject));
        }
        
        if (deweyDecimals != null)
        {
            foreach (var code in deweyDecimals.Distinct())
            {
                var dewey = DeweyDecimal.Create(code);
                book._deweyDecimals.Add(dewey);
            }
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
            foreach (var author in authors)
            {
                book._authors.Add(author);
            }
        }

        if (subjects != null)
        {
            foreach (var subject in subjects)
            {
                book._subjects.Add(subject);
            }
        }

        if (deweyDecimals != null)
        {
            foreach (var dewey in deweyDecimals)
            {
                book._deweyDecimals.Add(dewey);
            }
        }

        return book;
    }
    
    public void SetPublisher(Publisher publisher)
    {
        Publisher = publisher;
    }
}

