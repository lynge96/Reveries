using Reveries.Core.Models;
using Spectre.Console;

namespace Reveries.Console.Services.Interfaces;

public interface IBookDisplayService
{
    void DisplayBooksTree(List<Book> books);
    void DisplayBooksTable(List<Book> books);
}
