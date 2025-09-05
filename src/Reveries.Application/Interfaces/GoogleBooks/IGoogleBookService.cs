using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.GoogleBooks;

public interface IGoogleBookService
{
    Task<Book?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
}