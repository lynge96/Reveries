namespace Reveries.Core.Entities;

public class Subject
{
    public int SubjectId { get; set; }
    
    public required string Name { get; set; }
    
    public DateTimeOffset DateCreated { get; set; }
}
