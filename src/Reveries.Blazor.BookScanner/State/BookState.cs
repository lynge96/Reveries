using Reveries.Contracts.Books;

namespace Reveries.Blazor.BookScanner.State;

public class BookState
{
    public BookDto? CurrentBook { get; private set; }

    public void SetBook(BookDto book) => CurrentBook = book;

    public void Clear() => CurrentBook = null;
}