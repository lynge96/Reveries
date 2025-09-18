namespace Reveries.Core.Entities;

public class AuthorNameVariant
{
    public int Id { get; set; }
    
    public string NameVariant { get; set; }
    
    public bool IsPrimary { get; set; }
    
    public DateTimeOffset DateCreated { get; set; }
}
