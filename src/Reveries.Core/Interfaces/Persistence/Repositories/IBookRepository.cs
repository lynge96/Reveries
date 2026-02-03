using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Core.Interfaces.Persistence.Repositories;

public interface IBookRepository
{
    Task<List<Book>> GetBooksByAuthorAsync(string authorName);
    Task<List<Book>> GetBooksByAuthorsAsync(IEnumerable<string> authorNames);
    Task<List<Book>> GetBooksByPublisherAsync(string publisherName);
    Task<List<Book>> GetDetailedBooksByTitleAsync(List<string> bookTitles);
    Task<List<Book>> GetDetailedBooksByIsbnsAsync(IEnumerable<Isbn> isbns);
    Task<Book?> GetBookByIsbnAsync(Isbn? isbn13, Isbn? isbn10 = null);
    Task<Book?> GetBookByIdAsync(int id);
    Task<Book> CreateAsync(Book book);
    Task<List<Book>> GetAllBooksAsync();
    Task UpdateBookAsync(Book book);
}