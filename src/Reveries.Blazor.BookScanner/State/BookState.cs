using Reveries.Blazor.BookScanner.ApiContracts;

namespace Reveries.Blazor.BookScanner.State;

public class BookState
{
    public BookResponse? CurrentBook { get; private set; }

    public void SetBook(BookResponse bookDetails) => CurrentBook = bookDetails;

    public void Clear() => CurrentBook = null;
}
