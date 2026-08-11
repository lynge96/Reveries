namespace Reveries.Infrastructure.Postgresql.Entities;

public sealed class AuthorEntity
{
    public Guid Id { get; set; }
    public string NormalizedName { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTimeOffset? DateCreated { get; set; }
    public List<AuthorNameVariantEntity>? AuthorNameVariants { get; set; }
}