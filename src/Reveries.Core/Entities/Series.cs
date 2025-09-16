namespace Reveries.Core.Entities;

public class Series
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    
    public DateTimeOffset DateCreated { get; set; }

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