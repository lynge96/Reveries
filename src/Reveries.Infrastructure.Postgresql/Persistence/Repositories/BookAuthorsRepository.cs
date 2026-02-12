using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Core.ValueObjects.DTOs;
using Reveries.Infrastructure.Postgresql.Interfaces;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class BookAuthorsRepository : IBookAuthorsRepository
{
    private readonly IDbContext _dbContext;
    
    public BookAuthorsRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(int bookId, IEnumerable<AuthorWithId> authors)
    {
        const string sql = """
                           INSERT INTO library.books_authors (book_id, author_id)
                           VALUES (@BookId, @AuthorId)
                           ON CONFLICT DO NOTHING;
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        var parameters = authors
            .Select(a => new { BookId = bookId, AuthorId = a.DbId });
        
        await connection.ExecuteAsync(sql, parameters);
    }
}
