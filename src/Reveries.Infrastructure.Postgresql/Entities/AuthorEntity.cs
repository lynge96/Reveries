namespace Reveries.Infrastructure.Postgresql.Entities;

public class AuthorEntity
{
    public int AuthorId { get; set; }
    public string NormalizedName { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTimeOffset DateCreatedAuthor { get; set; }
}