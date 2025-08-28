namespace Reveries.Core.Entities;

public class Series
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    
    public DateTimeOffset DateCreated { get; set; }
}