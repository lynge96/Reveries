using Reveries.Core.Entities;

namespace Reveries.Console.Services.Interfaces;

public interface IBookSelectionService
{
    List<Book> SelectBooksToSave(List<Book> books);
    List<Book> FilterBooksByLanguage(IEnumerable<Book> books);
}
