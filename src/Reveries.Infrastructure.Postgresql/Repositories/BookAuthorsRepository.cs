using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Infrastructure.Postgresql.Interfaces;

namespace Reveries.Infrastructure.Postgresql.Repositories;

public class BookAuthorsRepository : IBookAuthorsRepository
{
    private readonly IDbContext _dbContext;
    
    public BookAuthorsRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertBookAuthorsAsync(
        Guid bookId,
        IEnumerable<Guid> authorIds,
        CancellationToken ct)
    {
        const string sql = """
                           INSERT INTO library.books_authors (book_id, author_id)
                           VALUES (@BookId, @AuthorId)
                           ON CONFLICT (book_id, author_id) DO NOTHING
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var parameters = authorIds.Select(authorId => new 
        { 
            BookId = bookId, 
            AuthorId = authorId 
        });
    
        var command = new CommandDefinition(
            commandText: sql,
            parameters: parameters,
            cancellationToken: ct
        );

        await connection.ExecuteAsync(command);
    }
}
