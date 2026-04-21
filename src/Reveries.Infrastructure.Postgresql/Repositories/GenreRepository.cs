using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Postgresql.Entities;
using Reveries.Infrastructure.Postgresql.Interfaces;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly IDbContext _dbContext;
    
    public GenreRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> AddAsync(Genre genre)
    {
        const string sql = """
                           INSERT INTO library.genres (name)
                           VALUES (@Name)
                           ON CONFLICT DO NOTHING
                           RETURNING id
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();

        var genreEntity = genre.ToDbModel();
        
        var genreDbId = await connection.QuerySingleAsync<int>(sql, genreEntity);

        return genreDbId;
    }
    
    public async Task<Genre?> GetByNameAsync(string genreName)
    {
        const string sql = """
                           SELECT 
                               id, 
                               name, 
                               date_created
                           FROM library.genres 
                           WHERE name = @Name
                           LIMIT 1
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
    
        var row = await connection.QueryFirstOrDefaultAsync<GenreEntity>(sql, new { Genre = genreName });

        return row?.ToDomain();
    }
    
    public async Task<IReadOnlyList<Genre>> GetByNamesAsync(IEnumerable<string> names)
    {
        const string sql = """
                           SELECT 
                               id,
                               name,
                               date_created
                           FROM library.genres
                           WHERE name = ANY(@Names);
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
        
        var rows = await connection.QueryAsync<GenreEntity>(sql, new { Names = names.ToArray() });
        
        return rows.Select(r => r.ToDomain()).ToList();
    }
}