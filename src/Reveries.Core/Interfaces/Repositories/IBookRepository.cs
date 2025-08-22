using Reveries.Core.Entities;

namespace Reveries.Core.Interfaces.Repositories;

public interface IBookRepository
{
    Task<List<Book>> GetBooksByAuthorAsync(string authorName);
    Task<List<Book>> GetBooksByAuthorsAsync(IEnumerable<string> authorNames);
    Task<List<Book>> GetBooksByPublisherAsync(string publisherName);
    Task<List<Book>> GetBooksWithDetailsByTitlesAsync(List<string> bookTitles);
    Task<List<Book>> GetBooksWithDetailsByIsbnAsync(IEnumerable<string> isbns);
    Task<Book?> GetBookByIsbnAsync(string? isbn13, string? isbn10 = null);
    Task<int> CreateBookAsync(Book book);
}