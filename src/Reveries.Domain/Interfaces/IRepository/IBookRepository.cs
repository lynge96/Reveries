using Reveries.Domain.Authors;
using Reveries.Domain.Books;
using Reveries.Domain.Publishers;
using Reveries.Domain.Shared;

namespace Reveries.Domain.Interfaces.IRepository;

public interface IBookRepository
{
    Task InsertBookAsync(Book book, CancellationToken ct);
    Task<Book?> GetBookByIsbnAsync(Isbn? isbn13, Isbn? isbn10 = null, CancellationToken ct = default);
    Task<bool> BookExistsAsync(Isbn isbn, CancellationToken ct = default);
    Task UpdateBookSeriesAsync(Book book, Guid seriesId, CancellationToken ct = default);

    Task<List<Book>> GetBooksByAuthorAsync(Author author, CancellationToken ct = default);
    Task<List<Book>> GetBooksByAuthorsAsync(IEnumerable<Author> authors, CancellationToken ct = default);
    Task<List<Book>> GetBooksByPublisherAsync(Publisher publisher, CancellationToken ct = default);
    Task<List<Book>> GetDetailedBooksByTitleAsync(List<Title> bookTitles, CancellationToken ct = default);
    Task<List<Book>> GetDetailedBooksByIsbnsAsync(IEnumerable<Isbn> isbns, CancellationToken ct = default);
    Task<Book?> GetBookByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Book>> GetAllBooksAsync(CancellationToken ct = default);

}
