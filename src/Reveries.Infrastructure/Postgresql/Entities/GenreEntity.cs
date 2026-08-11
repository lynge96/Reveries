namespace Reveries.Infrastructure.Postgresql.Entities;

public sealed class GenreEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTimeOffset? DateCreated { get; set; }
}