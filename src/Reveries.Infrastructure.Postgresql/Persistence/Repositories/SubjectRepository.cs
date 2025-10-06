using Dapper;
using Reveries.Application.Interfaces.Persistence;
using Reveries.Core.Interfaces.Persistence.Repositories;
using Reveries.Core.Entities;
using Reveries.Infrastructure.Postgresql.DTOs;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly IDbContext _dbContext;
    
    public SubjectRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Subject?> GetSubjectByNameAsync(string genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
            return null;
        
        const string sql = """
                           SELECT id AS subjectId, genre, date_created AS dateCreatedSubject
                           FROM subjects 
                           WHERE genre = @Genre
                           LIMIT 1
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
    
        var subjectDto = await connection.QuerySingleOrDefaultAsync<SubjectDto>(sql, new { Genre = genre });
        
        return subjectDto?.ToDomain();
    }

    public async Task<int> CreateSubjectAsync(Subject subject)
    {
        const string sql = """
                           INSERT INTO subjects (genre)
                           VALUES (@Genre)
                           RETURNING id
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();

        var subjectDto = subject.ToDto();

        var subjectId = await connection.QuerySingleAsync<int>(sql, subjectDto);
        
        subject.Id = subjectId;
        return subjectId;
    }

}