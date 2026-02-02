using Dapper;
using Reveries.Application.Interfaces.Persistence;
using Reveries.Core.Interfaces.Persistence.Repositories;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class BookSubjectsRepository : IBookSubjectsRepository
{
    private readonly IDbContext _dbContext;
    
    public BookSubjectsRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveBookSubjectsAsync(int? bookId, IEnumerable<Genre> subjects)
    {
        const string sql = """
                           INSERT INTO books_subjects (book_id, subject_id)
                           VALUES (@BookId, @SubjectId)
                           ON CONFLICT DO NOTHING;
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        var parameters = subjects
            .Select(s => new { BookId = bookId, SubjectId = s.Id })
            .ToList();

        if (parameters.Count > 0)
        {
            await connection.ExecuteAsync(sql, parameters);
        }
    }

}
