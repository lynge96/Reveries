using Reveries.Core.Entities;

namespace Reveries.Core.Interfaces.Repositories;

public interface IBookRepository
{
    Task<Book?> GetBookByIsbnAsync(string? isbn13, string? isbn10 = null);
    Task<int> CreateBookAsync(Book book);
}