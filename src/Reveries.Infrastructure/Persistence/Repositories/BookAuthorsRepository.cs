using Dapper;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces.Repositories;
using Reveries.Infrastructure.Interfaces.Persistence;

namespace Reveries.Infrastructure.Persistence.Repositories;

public class BookAuthorsRepository : IBookAuthorsRepository
{
    private readonly IPostgresDbContext _dbContext;
    
    public BookAuthorsRepository(IPostgresDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveBookAuthorsAsync(int bookId, IEnumerable<Author> authors)
    {
        const string sql = """
                          INSERT INTO books_authors (book_id, author_id)
                          VALUES (@BookId, @AuthorId)
                          """;
        
        var connection = await _dbContext.GetConnectionAsync();
        await connection.ExecuteAsync(sql, 
            authors.Select(a => new { BookId = bookId, AuthorId = a.AuthorId }));
    }
}
