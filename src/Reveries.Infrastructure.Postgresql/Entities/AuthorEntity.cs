namespace Reveries.Infrastructure.Postgresql.Entities;

public sealed class AuthorEntity
{
    public int AuthorId { get; set; }
    public Guid AuthorDomainId { get; set; }
    public string NormalizedName { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateCreatedAuthor { get; set; }
}