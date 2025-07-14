using Reveries.Core.Models;

namespace Reveries.Application.Interfaces.Services;

public interface IBookService
{
    Task<BooksListResponse?> GetBooksByIsbnStringAsync(string isbnString);
    
    Task<List<Book?>> GetBookByTitleAsync(string title, string? languageCode);
}