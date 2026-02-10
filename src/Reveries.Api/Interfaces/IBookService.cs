using Reveries.Contracts.Books;
using Reveries.Contracts.DTOs;
using Reveries.Core.Identity;
using Reveries.Core.ValueObjects;

namespace Reveries.Api.Interfaces;

public interface IBookService
{
    Task<BookDetailsDto?> GetBookByIsbnAsync(Isbn isbn, CancellationToken ct = default);
    Task<IEnumerable<BookDetailsDto>> GetBooksByIsbnsAsync(List<Isbn> isbns, CancellationToken ct = default);
    Task<BookDetailsDto?> GetBookByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<BookDetailsDto>> GetAllBooksAsync(CancellationToken ct = default);
    Task<Guid> CreateBookAsync(CreateBookRequest bookRequest, CancellationToken ct = default);
}