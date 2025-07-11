using Reveries.Core.Models;

namespace Reveries.Application.Interfaces.Services;

public interface IBookService
{
    Task<Book?> GetBookByIsbnAsync(string isbn);
    
    Task<List<Book?>> GetBookByTitleAsync(string title, string languageCode);
    
    Task<BooksListResponse?> GetBooksByIsbnsAsync(List<string> isbns);
}