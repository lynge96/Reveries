using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Core.Interfaces.IRepository;

public interface IBookRepository
{
    Task InsertBookAsync(Book book, CancellationToken ct);
    Task<Book?> GetBookByIsbnAsync(Isbn? isbn13, Isbn? isbn10 = null, CancellationToken ct = default);
    Task<bool> BookExistsAsync(Isbn isbn, CancellationToken ct = default);
    Task UpdateBookSeriesAsync(Book book, Guid seriesId, CancellationToken ct = default);
    Task UpdateBookReadStatusAsync(Book book, CancellationToken ct = default);
    
    Task<List<Book>> GetBooksByAuthorAsync(Author author, CancellationToken ct = default);
    Task<List<Book>> GetBooksByAuthorsAsync(IEnumerable<Author> authors, CancellationToken ct = default);
    Task<List<Book>> GetBooksByPublisherAsync(Publisher publisher, CancellationToken ct = default);
    Task<List<Book>> GetDetailedBooksByTitleAsync(List<string> bookTitles, CancellationToken ct = default);
    Task<List<Book>> GetDetailedBooksByIsbnsAsync(IEnumerable<Isbn> isbns, CancellationToken ct = default);
    Task<Book?> GetBookByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Book>> GetAllBooksAsync(CancellationToken ct = default);
    
}