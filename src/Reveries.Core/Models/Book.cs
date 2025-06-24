namespace Reveries.Core.Models;

public class Book
{
    public int? Id { get; set; }
    
    public string? Isbn13 { get; set; }
    
    public required string Isbn10 { get; set; }
    
    public required string Title { get; set; }

    public required List<string> Authors { get; init; } = new();
    
    public int? Pages { get; set; }
    
    public bool IsRead { get; set; }
    
    public string? Publisher { get; set; }
    
    public string? Language { get; set; }
    
    public DateTime? PublishDate { get; set; }
    
    public string? Synopsis { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public decimal? Msrp { get; set; }
    
    public string? Binding { get; set; }

    public List<string> Subjects { get; set; } = new();
    
    public DimensionsStructured? Dimensions { get; set; }
    
    public DateTime DateCreated  { get; set; }
    
    
    public override string ToString()
    {
        var title = string.IsNullOrEmpty(Title) ? "Unknown Title" : Title;
        var authors = Authors.Count == 0 ? "Unknown Author" : string.Join(", ", Authors);

        return $"{title} by {authors}";
    }
}

