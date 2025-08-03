using Reveries.Core.Entities;

namespace Reveries.Core.Interfaces;

public interface IBookRepository
{
    Task<Book?> GetBookByIsbnAsync(string isbn);
    Task<int> CreateBookAsync(Book book);
}