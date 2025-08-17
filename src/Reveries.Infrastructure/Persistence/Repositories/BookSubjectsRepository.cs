using Dapper;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces.Repositories;
using Reveries.Infrastructure.Interfaces.Persistence;

namespace Reveries.Infrastructure.Persistence.Repositories;

public class BookSubjectsRepository : IBookSubjectsRepository
{
    private readonly IPostgresDbContext _dbContext;
    
    public BookSubjectsRepository(IPostgresDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveBookSubjectsAsync(int bookId, IEnumerable<Subject> subjects)
    {
        const string sql = """
                           INSERT INTO books_subjects (book_id, subject_id)
                           VALUES (@BookId, @SubjectId)
                           """;
    
        var connection = await _dbContext.GetConnectionAsync();
        await connection.ExecuteAsync(sql, 
            subjects.Select(s => new { BookId = bookId, SubjectId = s.Id }));
    }

}
