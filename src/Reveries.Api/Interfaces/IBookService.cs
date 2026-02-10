using Reveries.Application.Queries;
using Reveries.Contracts.Books;
using Reveries.Core.Identity;
using Reveries.Core.ValueObjects;

namespace Reveries.Api.Interfaces;

public interface IBookService
{
    Task<BookDetailsReadModel?> GetBookByIsbnAsync(Isbn isbn, CancellationToken ct = default);
    Task<IEnumerable<BookDetailsReadModel>> GetBooksByIsbnsAsync(List<Isbn> isbns, CancellationToken ct = default);
    Task<BookDetailsReadModel?> GetBookByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<BookDetailsReadModel>> GetAllBooksAsync(CancellationToken ct = default);
    Task<Guid> CreateBookAsync(CreateBookRequest bookRequest, CancellationToken ct = default);
}