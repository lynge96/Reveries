namespace Reveries.Core.Entities;

public class AuthorNameVariant
{
    public int Id { get; set; }
    
    public int AuthorId { get; set; }
    
    public string NameVariant { get; set; }
    
    public bool IsPrimary { get; set; }
    
    public Author Author { get; set; }
}
