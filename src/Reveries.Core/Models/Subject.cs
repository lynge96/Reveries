namespace Reveries.Core.Models;

public class Subject
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
