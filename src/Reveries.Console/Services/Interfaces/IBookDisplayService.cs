using Reveries.Core.Entities;
using Spectre.Console;

namespace Reveries.Console.Services.Interfaces;

public interface IBookDisplayService
{
    Tree DisplayBooks(List<Book> books);
}
