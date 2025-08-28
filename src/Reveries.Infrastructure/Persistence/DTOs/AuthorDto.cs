namespace Reveries.Infrastructure.Persistence.DTOs;

public class AuthorDto
{
    public int AuthorId { get; set; }
    public string NormalizedName { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateCreatedAuthor { get; set; }
}