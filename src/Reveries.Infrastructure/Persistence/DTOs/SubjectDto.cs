namespace Reveries.Infrastructure.Persistence.DTOs;

public class SubjectDto
{
    public int SubjectId { get; set; }
    public string Genre { get; set; } = null!;
    public DateTime DateCreatedSubject { get; set; }
}