namespace Reveries.Core.Models;

public class Subject
{
    public int Id { get; set; }
    
    public required string Genre { get; set; }
    
    public DateTimeOffset DateCreated { get; set; }

    public override string ToString()
    {
        return Genre;
    }
}

