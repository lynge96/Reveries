namespace Reveries.Core.Models;

public class BooksListResponse
{
    public int Total { get; init; }
    public string Requested { get; init; }
    public List<Book> Books { get; init; }

    public BooksListResponse(int total, string requested, List<Book> books)
    {
        Total = total;
        Requested = requested;
        Books = books;
    }

}