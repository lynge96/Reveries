using Reveries.Core.Models;

namespace Reveries.Infrastructure.Postgresql.Interfaces;

public interface IBookAuthorsRepository
{
    Task SaveBookAuthorsAsync(int? bookId, IEnumerable<Author> authors);
}