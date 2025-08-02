namespace Reveries.Core.Entities;

public class Author
{
    public int Id { get; set; }
    
    public required string NormalizedName { get; init; }
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }

    public ICollection<string> NameVariants { get; set; } = new List<string>();
    
    public ICollection<Book> Books { get; set; } = new List<Book>();

    public DateTimeOffset DateCreated { get; set; }
}
