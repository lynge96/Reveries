using Reveries.Core.Models;

namespace Reveries.Core.ValueObjects.DTOs;

public record AuthorWithId
{
    public Author Author { get; init; }
    public int DbId { get; init; }
    
    public AuthorWithId(Author author, int dbId)
    {
        Author = author;
        DbId = dbId;
    }
}