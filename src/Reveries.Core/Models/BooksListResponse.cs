namespace Reveries.Core.Models;

public class BooksListResponse
{
    public int Total { get; init; }
    public int Requested { get; init; }
    public List<Book> Books { get; init; }

    public BooksListResponse(int total, int requested, List<Book> books)
    {
        Total = total;
        Requested = requested;
        Books = books;
    }

}