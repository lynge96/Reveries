using Dapper;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces;
using Reveries.Infrastructure.Persistence.Context;

namespace Reveries.Infrastructure.Persistence.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IPostgresDbContext _dbContext;
    
    public BookRepository(IPostgresDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Book?> GetBookByIsbnAsync(string isbn)
    {
        const string sql = """
                           SELECT *
                           FROM books 
                           WHERE isbn13 = @Isbn 
                              OR isbn10 = @Isbn
                           LIMIT 1
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
        
        return await connection.QuerySingleOrDefaultAsync<Book>(sql, new { Isbn = isbn });
    }

    public Task<int> CreateBookAsync(Book book)
    {
        throw new NotImplementedException();
    }
}