namespace Reveries.Infrastructure.Postgresql.DTOs;

public class SubjectDto
{
    public int SubjectId { get; set; }
    public string Genre { get; set; } = null!;
    public DateTimeOffset DateCreatedSubject { get; set; }
}