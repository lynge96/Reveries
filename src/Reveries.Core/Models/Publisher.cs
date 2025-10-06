namespace Reveries.Core.Models;

public class Publisher
{
    public int Id { get; set; }
    
    public string? Name { get; set; }
    
    public DateTimeOffset DateCreated { get; set; }

    public override string? ToString()
    {
        return Name;
    }
}
