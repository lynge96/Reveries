namespace Reveries.Core.Models;

public class Subject : BaseEntity
{
    public int Id { get; set; }
    
    public required string Genre { get; set; }

    public override string ToString()
    {
        return Genre;
    }
}

