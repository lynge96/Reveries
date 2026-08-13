using Dapper;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Domain.Models;
using Reveries.Persistence.Entities;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Mappers;

namespace Reveries.Persistence.Repositories;

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

        var command = _dbContext.CreateCommand(sql, publisherEntity, ct);

        var result = await connection.QuerySingleAsync<PublisherEntity>(command);

        return result.ToDomain();
    }

    public async Task<List<Publisher>> SearchByNameAsync(Publisher publisher, CancellationToken ct)
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

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, new { Name = $"%{publisher.Name}%" }, ct);

        var rows = await connection.QueryAsync<PublisherEntity>(command);
        
        return rows.Select(r => r.ToDomain()).ToList();
    }
}