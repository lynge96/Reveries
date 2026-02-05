using Dapper;
using Reveries.Infrastructure.Postgresql.Entities;
using Reveries.Infrastructure.Postgresql.Interfaces;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly IDbContext _dbContext;
    
    public GenreRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> AddAsync(GenreEntity genre)
    {
        const string sql = """
                           INSERT INTO library.genres (name)
                           VALUES (@Name)
                           ON CONFLICT DO NOTHING
                           RETURNING id
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();

        var genreDbId = await connection.QuerySingleAsync<int>(sql, genre);

        return genreDbId;
    }
    
    public async Task<GenreEntity?> GetByNameAsync(string genreName)
    {
        const string sql = """
                           SELECT 
                               id AS genreId, 
                               name, 
                               date_created AS dateCreatedGenre
                           FROM library.genres 
                           WHERE name = @Name
                           LIMIT 1
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
    
        var genreDto = await connection.QueryFirstOrDefaultAsync<GenreEntity>(sql, new { Genre = genreName });
        
        return genreDto;
    }
    
}