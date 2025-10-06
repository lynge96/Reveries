namespace Reveries.Infrastructure.Postgresql.Entities;

public class SubjectEntity
{
    public int SubjectId { get; set; }
    public string Genre { get; set; } = null!;
    public DateTimeOffset DateCreatedSubject { get; set; }
}