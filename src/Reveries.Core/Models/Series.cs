namespace Reveries.Core.Models;

public class Series : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public Series()
    {
        
    }
    public Series(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}