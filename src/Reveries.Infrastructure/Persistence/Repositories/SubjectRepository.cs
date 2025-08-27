using Dapper;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces;
using Reveries.Core.Interfaces.Repositories;
using Reveries.Infrastructure.Interfaces.Persistence;
using Reveries.Infrastructure.Persistence.Context;

namespace Reveries.Infrastructure.Persistence.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly IPostgresDbContext _dbContext;
    
    public SubjectRepository(IPostgresDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Subject?> GetSubjectByNameAsync(string genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
            return null;
        
        const string sql = """
                           SELECT id AS subjectId, genre, date_created AS datecreatedsubject 
                           FROM subjects 
                           WHERE genre = @Genre
                           LIMIT 1
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
    
        return await connection.QuerySingleOrDefaultAsync<Subject>(sql, new { Genre = genre });
    }

    public async Task<int> CreateSubjectAsync(Subject subject)
    {
        const string sql = """
                           INSERT INTO subjects (genre, date_created)
                           VALUES (@Genre, @DateCreated)
                           RETURNING id
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();

        var subjectId = await connection.QuerySingleAsync<int>(sql, 
            new {
                Genre = subject.Genre,
                DateCreated = DateTimeOffset.UtcNow 
            });
        
        subject.SubjectId = subjectId;
    
        return subjectId;
    }

}