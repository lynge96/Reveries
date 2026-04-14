namespace Reveries.Core.ValueObjects.DTOs;

public record GenreWithId
{
    public Genre Genre { get; init; }
    public int DbId { get; init; }

    public GenreWithId(Genre genre, int dbId)
    {
        Genre = genre;
        DbId = dbId;
    }
}