using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.GoogleBooks;

public interface IGoogleBookService
{
    Task<List<Book>> GetBooksByIsbnsAsync(List<string> isbns, CancellationToken cancellationToken = default);
    Task<List<Book>> GetBooksByTitleAsync(List<string> titles, CancellationToken cancellationToken = default);
}