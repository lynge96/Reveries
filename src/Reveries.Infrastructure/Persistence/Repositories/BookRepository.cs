using Reveries.Core.Entities;
using Reveries.Core.Interfaces;
using Reveries.Infrastructure.Persistence.ConnectionFactory;

namespace Reveries.Infrastructure.Persistence.Repositories;

public class BookRepository : BaseRepository, IBookRepository
{
    public BookRepository(IPostgresConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task<Book?> GetBookByIsbnAsync(string isbn)
    {
        const string sql = """
                           SELECT * FROM books 
                           WHERE isbn = @Isbn
                           """;
            
        return await QuerySingleOrDefaultAsync<Book>(sql, new { Isbn = isbn });
    }

    public Task<int> CreateBookAsync(Book book)
    {
        throw new NotImplementedException();
    }
}