using Reveries.Core.Models;

namespace Reveries.Core.ValueObjects.DTOs;

public class BookWithId
{
    public Book Book { get; init; }
    public int DbId { get; init; }

    public BookWithId(Book book, int dbId)
    {
        Book = book;
        DbId = dbId;
    }
}