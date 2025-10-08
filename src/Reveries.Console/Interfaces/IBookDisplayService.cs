using Reveries.Core.Models;

namespace Reveries.Console.Interfaces;

public interface IBookDisplayService
{
    void DisplayBooksTree(List<Book> books);
    void DisplayBooksTable(List<Book> books);
}
