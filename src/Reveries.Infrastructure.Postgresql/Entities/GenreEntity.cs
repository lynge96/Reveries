namespace Reveries.Infrastructure.Postgresql.Entities;

public sealed class GenreEntity
{
    public int GenreId { get; set; }
    public string Name { get; set; } = null!;
    public DateTime DateCreatedGenre { get; set; }
}