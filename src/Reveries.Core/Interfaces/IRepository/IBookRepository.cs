using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Core.Interfaces.IRepository;

public interface IBookRepository
{
    Task<int> AddAsync(Book book, int? publisherId, int? seriesId);
    Task<Book?> GetBookByIsbnAsync(string? isbn13, string? isbn10 = null);
    
    Task<List<Book>> GetBooksByAuthorAsync(string authorName);
    Task<List<Book>> GetBooksByAuthorsAsync(IEnumerable<string> authorNames);
    Task<List<Book>> GetBooksByPublisherAsync(string publisherName);
    Task<List<Book>> GetDetailedBooksByTitleAsync(List<string> bookTitles);
    Task<List<Book>> GetDetailedBooksByIsbnsAsync(IEnumerable<Isbn> isbns);
    Task<Book?> GetBookByIdAsync(int id);
    Task<List<Book>> GetAllBooksAsync();
    Task UpdateBookSeriesAsync(Book book);
}