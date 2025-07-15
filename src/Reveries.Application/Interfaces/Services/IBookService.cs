using Reveries.Core.Models;

namespace Reveries.Application.Interfaces.Services;

public interface IBookService
{
    Task<BooksListResponse> GetBooksByIsbnStringAsync(string isbnString, CancellationToken cancellationToken = default);
    
    Task<List<Book>> GetBooksByTitleAsync(string title, string? languageCode, CancellationToken cancellationToken = default);
}