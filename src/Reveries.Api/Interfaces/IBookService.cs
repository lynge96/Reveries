using Reveries.Contracts.DTOs;

namespace Reveries.Api.Interfaces;

public interface IBookService
{
    Task<BookDto?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
    Task<BookDto?> GetBookByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookDto>> GetAllBooksAsync(CancellationToken cancellationToken = default);
    Task<int> CreateBookAsync(CreateBookDto bookDto, CancellationToken cancellationToken = default);
}