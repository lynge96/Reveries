using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Core.ValueObjects;
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

            var command = new CommandDefinition(
                commandText: sql,
                parameters: genreEntity,
                cancellationToken: ct
            );
        
            var genreDbId = await connection.QuerySingleAsync<int>(command);
            
            genreIds.Add(genreDbId);
        }
        
        return genreIds;
    }

}