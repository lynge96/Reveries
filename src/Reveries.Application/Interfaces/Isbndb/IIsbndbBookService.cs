using Reveries.Core.Entities;
using Reveries.Core.Enums;

namespace Reveries.Application.Interfaces.Isbndb;

public interface IIsbndbBookService
{
    Task<List<Book>> GetBooksByIsbnsAsync(List<string> isbns, CancellationToken cancellationToken = default);
    Task<List<Book>> GetBooksByTitlesAsync(List<string> titles, string? languageCode, BookFormat format, CancellationToken cancellationToken = default);
}