namespace Reveries.Infrastructure.Postgresql.Entities;

public sealed class AuthorNameVariantEntity
{
    public int VariantId { get; set; }
    public int AuthorId { get; set; }
    public string? NameVariant { get; set; }
    public bool IsPrimary { get; set; }
}