using Dapper;
using Reveries.Application.Interfaces.Persistence;
using Reveries.Core.Models;
using Reveries.Infrastructure.Postgresql.Interfaces;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class BookAuthorsRepository : IBookAuthorsRepository
{
    private readonly IDbContext _dbContext;
    
    public BookAuthorsRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveBookAuthorsAsync(int? bookId, IEnumerable<Author> authors)
    {
        const string sql = """
                           INSERT INTO books_authors (book_id, author_id)
                           VALUES (@BookId, @AuthorId)
                           ON CONFLICT DO NOTHING;
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        var parameters = authors
            .Select(a => new { BookId = bookId, AuthorId = a.Id })
            .ToList();

        if (parameters.Count > 0)
        {
            await connection.ExecuteAsync(sql, parameters);
        }
    }
}
