using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Interfaces;

public interface IAuthorRepository
{
    Task<int> AddAsync(AuthorEntity author);
    Task<AuthorEntity?> GetByNameAsync(string authorName);
    Task<List<AuthorEntity>> GetAuthorsByNameAsync(string name);
}