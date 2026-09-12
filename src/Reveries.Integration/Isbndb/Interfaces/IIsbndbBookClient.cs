using Reveries.Domain.Editions;
using Reveries.Integration.Isbndb.Dtos;

namespace Reveries.Integration.Isbndb.Interfaces;

public interface IIsbndbBookClient
{
    Task<IsbndbBookResponseDto?> FetchBookByIsbnAsync(Isbn isbn, CancellationToken ct = default);
    Task<IsbndbBookListResponseDto?> FetchBooksByIsbnsAsync(IEnumerable<Isbn> isbns, CancellationToken ct = default);
}