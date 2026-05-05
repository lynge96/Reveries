using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Core.Models;
using Reveries.Infrastructure.Postgresql.Entities;
using Reveries.Infrastructure.Postgresql.Interfaces;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Repositories;

public class PublisherRepository : IPublisherRepository
{
    private readonly IDbContext _dbContext;
    
    public PublisherRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Publisher?> GetOrCreateAsync(
        Publisher? publisher,
        CancellationToken ct)
    {
        if (publisher is null)
            return null;
    
        const string sql = """
                           INSERT INTO library.publishers (id, name)
                           VALUES (@Id, @Name)
                           ON CONFLICT (name) DO UPDATE 
                           SET name = EXCLUDED.name
                           RETURNING id, name, date_created
                           """;
    
        var connection = await _dbContext.GetConnectionAsync(ct);
        var publisherEntity = publisher.ToEntity();

        var command = new CommandDefinition(
            commandText: sql,
            parameters: publisherEntity,
            cancellationToken: ct
        );

        var result = await connection.QuerySingleAsync<PublisherEntity>(command);

        return result.ToDomain();
    }

    public async Task<List<Publisher>> SearchByNameAsync(string publisherName)
    {
        const string sql = """
                           SELECT 
                               id, 
                               name, 
                               date_created
                           FROM library.publishers
                           WHERE name ILIKE @Name
                           ORDER BY name
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        var command = new CommandDefinition(
            commandText: sql,
            parameters: new { Name = $"%{publisherName}%" }
        );
        
        var rows = await connection.QueryAsync<PublisherEntity>(command);
        
        return rows.Select(r => r.ToDomain()).ToList();
    }
}