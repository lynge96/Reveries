using Reveries.Contracts.DTOs;

namespace Reveries.Blazor.BookScanner.State;

public class BookState
{
    public BookDetailsDto? CurrentBook { get; private set; }

    public void SetBook(BookDetailsDto bookDetails) => CurrentBook = bookDetails;

    public void Clear() => CurrentBook = null;
}