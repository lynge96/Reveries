using Reveries.Core.Models;

namespace Reveries.Console.Interfaces;

public interface IBookSelectionService
{
    List<Book> SelectBooksToSave(List<Book> books);
    List<Book> FilterBooksByLanguage(IEnumerable<Book> books);
}
