namespace Reveries.Core.Entities;

public class Subject
{
    public int SubjectId { get; set; }
    
    public required string Genre { get; set; }
    
    public DateTimeOffset DateCreatedSubject { get; set; }
}
