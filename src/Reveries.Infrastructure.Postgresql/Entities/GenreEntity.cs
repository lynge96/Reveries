namespace Reveries.Infrastructure.Postgresql.Entities;

public sealed class GenreEntity
{
    public int GenreId { get; set; }
    public string GenreName { get; set; } = null!;
    public DateTimeOffset? DateCreatedGenre { get; set; }
}