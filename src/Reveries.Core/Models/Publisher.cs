namespace Reveries.Core.Models;

public class Publisher : BaseEntity
{
    public int Id { get; set; }
    
    public string? Name { get; set; }

    public override string? ToString()
    {
        return Name;
    }
}
