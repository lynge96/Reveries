using Reveries.Core.Models;

namespace Reveries.Core.Interfaces.Persistence.Repositories;

public interface IBookRepository
{
    Task<List<Book>> GetBooksByAuthorAsync(string authorName);
    Task<List<Book>> GetBooksByAuthorsAsync(IEnumerable<string> authorNames);
    Task<List<Book>> GetBooksByPublisherAsync(string publisherName);
    Task<List<Book>> GetDetailedBooksByTitleAsync(List<string> bookTitles);
    Task<List<Book>> GetDetailedBooksByIsbnsAsync(IEnumerable<string> isbns);
    Task<Book?> GetBookByIsbnAsync(string? isbn13, string? isbn10 = null);
    Task<Book?> GetBookByIdAsync(int id);
    Task<int> CreateAsync(Book book);
    Task<List<Book>> GetAllBooksAsync();
    Task UpdateBookAsync(Book book);
}