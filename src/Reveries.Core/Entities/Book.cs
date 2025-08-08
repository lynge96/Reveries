namespace Reveries.Core.Entities;

public class Book
{
    public int? Id { get; set; }
    
    public string? Isbn13 { get; set; }
    
    public string? Isbn10 { get; set; }
    
    public required string Title { get; set; }

    public ICollection<Author> Authors { get; set; } = new List<Author>();
    
    public int? Pages { get; set; }
    
    public bool IsRead { get; set; }
    
    public int? PublisherId { get; set; }

    public Publisher? Publisher { get; set; }
    
    public string? LanguageIso639 { get; set; }
    
    public string? Language { get; set; }
    
    public DateTime? PublishDate { get; set; }
    
    public string? Synopsis { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public decimal? Msrp { get; set; }
    
    public string? Binding { get; set; }

    public string? Edition { get; set; }
    
    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    
    public BookDimensions? Dimensions { get; set; }
    
    public DateTimeOffset DateCreated  { get; set; }
    
    public override string ToString()
    {
        var title = string.IsNullOrEmpty(Title) ? "Unknown Title" : Title;
        var authors = Authors
            .Select(a => string.Join(" ", new[] { a.FirstName, a.LastName }.Where(n => !string.IsNullOrEmpty(n))))
            .DefaultIfEmpty("Unknown Author")
            .ToList();
    
        return $"{title} by {string.Join(", ", authors)}";
    }
    
}

