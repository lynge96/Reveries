using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Core.Models;
using Reveries.Infrastructure.Postgresql.Interfaces;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class BookAuthorsRepository : IBookAuthorsRepository
{
    private readonly IDbContext _dbContext;
    
    public BookAuthorsRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveBookAuthorsAsync(int bookId, IEnumerable<Author> authors)
    {
        const string sql = """
                           INSERT INTO library.books_authors (book_id, author_id)
                           VALUES (@BookId, @AuthorId)
                           ON CONFLICT DO NOTHING;
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        var parameters = authors
            .Select(a => a.ToDbModel())
            .Select(a => new { BookId = bookId, a.AuthorId });
        
        await connection.ExecuteAsync(sql, parameters);
    }
}
