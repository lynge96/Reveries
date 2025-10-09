namespace Reveries.Core.Models;

public class AuthorNameVariant : BaseEntity
{
    public int Id { get; set; }
    
    public string NameVariant { get; set; }
    
    public bool IsPrimary { get; set; }
}
