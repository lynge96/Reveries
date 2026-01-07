using Reveries.Contracts.DTOs;

namespace Reveries.Api.Interfaces;

public interface IBookService
{
    Task<BookDto?> GetBookByIsbnAsync(string isbn, CancellationToken ct = default);
    Task<IEnumerable<BookDto>> GetBooksByIsbnsAsync(List<string> isbns, CancellationToken ct = default);
    Task<BookDto?> GetBookByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<BookDto>> GetAllBooksAsync(CancellationToken ct = default);
    Task<int> CreateBookAsync(CreateBookDto bookDto, CancellationToken ct = default);
}