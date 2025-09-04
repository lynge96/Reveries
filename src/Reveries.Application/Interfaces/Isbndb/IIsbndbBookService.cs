using Reveries.Core.Entities;
using Reveries.Core.Enums;

namespace Reveries.Application.Interfaces.Isbndb;

public interface IIsbndbBookService
{
    Task<List<Book>> GetBooksByIsbnStringAsync(List<string> isbns, CancellationToken cancellationToken = default);
    
    Task<List<Book>> GetBooksByTitleAsync(List<string> titles, string? languageCode, BookFormat format, CancellationToken cancellationToken = default);
}