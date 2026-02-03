using Reveries.Contracts.DTOs;
using Reveries.Core.Identity;
using Reveries.Core.ValueObjects;

namespace Reveries.Api.Interfaces;

public interface IBookService
{
    Task<BookDto?> GetBookByIsbnAsync(Isbn isbn, CancellationToken ct = default);
    Task<IEnumerable<BookDto>> GetBooksByIsbnsAsync(List<Isbn> isbns, CancellationToken ct = default);
    Task<BookDto?> GetBookByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<BookDto>> GetAllBooksAsync(CancellationToken ct = default);
    Task<Guid> CreateBookAsync(CreateBookDto bookDto, CancellationToken ct = default);
}