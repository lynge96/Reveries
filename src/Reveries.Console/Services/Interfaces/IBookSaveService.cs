using Reveries.Core.Entities;

namespace Reveries.Console.Services.Interfaces;

public interface IBookSaveService
{
    Task SaveBooksAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default);
}
