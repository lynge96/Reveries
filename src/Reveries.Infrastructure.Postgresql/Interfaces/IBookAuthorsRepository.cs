using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Interfaces;

public interface IBookAuthorsRepository
{
    Task SaveBookAuthorsAsync(int bookId, IEnumerable<AuthorEntity> authors);
}