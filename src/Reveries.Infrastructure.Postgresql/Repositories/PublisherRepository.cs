using System.Data;
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

    public async Task<Publisher?> GetOrCreateAsync(Publisher? publisher, IDbTransaction? transaction, CancellationToken ct)
    {
        if (publisher is null)
            return null;
        
        var existing = await GetByNameAsync(publisher.Name);
        if (existing != null)
            return existing;
        
        await InsertPublisherAsync(publisher, transaction, ct);
        
        return publisher;
    }

    private async Task InsertPublisherAsync(Publisher publisher, IDbTransaction? transaction = null, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO library.publishers (id, name)
                           VALUES (@Id, @Name)
                           ON CONFLICT (name) DO NOTHING
                           """;
        
        var connection = await _dbContext.GetConnectionAsync(ct);

        var publisherEntity = publisher.ToDbModel();
        
        await connection.QuerySingleAsync(
            sql, 
            publisherEntity,
            transaction
            );
    }
    
    public async Task<Publisher?> GetByNameAsync(string publisherName)
    {
        const string sql = """
                           SELECT 
                               id,
                               name, 
                               date_created
                           FROM library.publishers
                           WHERE name ILIKE @Name
                           LIMIT 1
                           """;

        var connection = await _dbContext.GetConnectionAsync();
        
        var row = await connection.QueryFirstOrDefaultAsync<PublisherEntity>(
            sql, 
            new { Name = publisherName }
            );

        return row?.ToDomain();
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
        
        var rows = await connection.QueryAsync<PublisherEntity>(
            sql, 
            new { Name = $"%{publisherName}%" }
            );
        
        return rows.Select(r => r.ToDomain()).ToList();
    }
}