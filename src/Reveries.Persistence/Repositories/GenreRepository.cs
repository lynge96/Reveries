using Dapper;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Domain.ValueObjects;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Mappers;

namespace Reveries.Persistence.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly IDbContext _dbContext;
    
    public GenreRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<int>> GetOrCreateGenresAsync(
        IReadOnlyList<Genre> genres,
        CancellationToken ct)
    {
        if (genres.Count == 0)
            return [];
        
        var genreIds = new List<int>();
        
        foreach (var genre in genres)
        {
            const string sql = """
                               INSERT INTO library.genres (name)
                               VALUES (@Name)
                               ON CONFLICT (name) DO UPDATE
                               SET name = EXCLUDED.name
                               RETURNING id
                               """;
        
            var connection = await _dbContext.GetConnectionAsync(ct);
            var genreEntity = genre.ToEntity();

            var command = _dbContext.CreateCommand(sql, genreEntity, ct);

            var genreDbId = await connection.QuerySingleAsync<int>(command);
            
            genreIds.Add(genreDbId);
        }
        
        return genreIds;
    }

}