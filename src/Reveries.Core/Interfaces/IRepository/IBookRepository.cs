using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Core.ValueObjects.DTOs;

namespace Reveries.Core.Interfaces.IRepository;

public interface IBookRepository
{
    Task<int> AddAsync(Book book, int? publisherId, int? seriesId);
    Task<BookWithId?> GetBookByIsbnAsync(Isbn? isbn13, Isbn? isbn10 = null);
    Task UpdateBookSeriesAsync(BookWithId book, int seriesId);
    Task UpdateBookReadStatusAsync(Book book);
    
    Task<List<Book>> GetBooksByAuthorAsync(string authorName);
    Task<List<Book>> GetBooksByAuthorsAsync(IEnumerable<string> authorNames);
    Task<List<Book>> GetBooksByPublisherAsync(string publisherName);
    Task<List<Book>> GetDetailedBooksByTitleAsync(List<string> bookTitles);
    Task<List<Book>> GetDetailedBooksByIsbnsAsync(IEnumerable<Isbn> isbns);
    Task<Book?> GetBookByIdAsync(int id);
    Task<List<Book>> GetAllBooksAsync();
    
}