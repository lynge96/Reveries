using Dapper;
using Reveries.Application.Interfaces.Persistence;
using Reveries.Core.Interfaces.Persistence.Repositories;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Postgresql.Entities;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly IDbContext _dbContext;
    
    public GenreRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Genre?> GetGenreByNameAsync(string genre)
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
    
        var subjectDto = await connection.QuerySingleOrDefaultAsync<GenreEntity>(sql, new { Genre = genre });
        
        return subjectDto?.ToDomain();
    }

    public async Task<Genre> CreateGenreAsync(Genre genre)
    {
        const string sql = """
                           INSERT INTO subjects (genre)
                           VALUES (@Genre)
                           RETURNING id
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();

        var subjectDto = genre.ToEntity();

        var subjectId = await connection.QuerySingleAsync<int>(sql, subjectDto);

        return genre;
    }

}