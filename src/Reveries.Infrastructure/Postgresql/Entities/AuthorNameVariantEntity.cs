namespace Reveries.Infrastructure.Postgresql.Entities;

public sealed class AuthorNameVariantEntity
{
    public int Id { get; set; }
    public Guid AuthorId { get; set; }
    public string? NameVariant { get; set; }
    public bool IsPrimary { get; set; }
}