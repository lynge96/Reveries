using Reveries.Core.Models;

namespace Reveries.Application.Interfaces.Services;

public interface IBookService
{
    Task<Book?> GetBookByIsbnAsync(string isbn);
}