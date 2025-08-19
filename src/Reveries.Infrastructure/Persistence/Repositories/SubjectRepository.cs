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

    public async Task<Subject?> GetSubjectByNameAsync(string name)
    {
        const string sql = """
                           SELECT * FROM subjects 
                           WHERE name = @Name
                           LIMIT 1
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
    
        return await connection.QuerySingleOrDefaultAsync<Subject>(sql, new { Name = name });
    }

    public async Task<int> CreateSubjectAsync(Subject subject)
    {
        const string sql = """
                           INSERT INTO subjects (name, date_created)
                           VALUES (@Name, @DateCreated)
                           RETURNING id
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();

        var subjectId = await connection.QuerySingleAsync<int>(sql, 
            new { 
                subject.Name,
                DateCreated = DateTimeOffset.UtcNow 
            });
        
        subject.SubjectId = subjectId;
    
        return subjectId;
    }

}