using Reveries.Core.Entities;
using Reveries.Core.Enums;

namespace Reveries.Application.Interfaces.Services;

public interface IBookService
{
    Task<List<Book>> GetBooksByIsbnStringAsync(string isbnString, CancellationToken cancellationToken = default);
    
    Task<List<Book>> GetBooksByTitleAsync(string title, string? languageCode, BookFormat format, CancellationToken cancellationToken = default);
}