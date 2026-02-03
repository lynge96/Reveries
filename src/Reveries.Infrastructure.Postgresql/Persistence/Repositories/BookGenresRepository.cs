using Dapper;
using Reveries.Application.Interfaces.Persistence;
using Reveries.Core.Interfaces.Persistence.Repositories;
using Reveries.Core.ValueObjects;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class BookGenresRepository : IBookGenresRepository
{
    private readonly IDbContext _dbContext;
    
    public BookGenresRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveBookGenresAsync(int? bookId, IEnumerable<Genre> genres)
    {
        const string sql = """
                           INSERT INTO books_subjects (book_id, subject_id)
                           VALUES (@BookId, @SubjectId)
                           ON CONFLICT DO NOTHING;
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        var parameters = genres
            .Select(s => new { BookId = bookId, SubjectId = s.Id })
            .ToList();

        if (parameters.Count > 0)
        {
            await connection.ExecuteAsync(sql, parameters);
        }
    }

}
