using Reveries.Contracts.Books;

namespace Reveries.Api.Interfaces;

public interface IBookService
{
    Task<BookDto?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookDto>> GetAllBooksAsync(CancellationToken cancellationToken = default);
}