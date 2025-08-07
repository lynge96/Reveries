using Dapper;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces;
using Reveries.Infrastructure.Persistence.Context;

namespace Reveries.Infrastructure.Persistence.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly IPostgresDbContext _dbContext;
    
    public AuthorRepository(IPostgresDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<int> CreateAuthorAsync(Author author)
    {
        const string sql = """
                           INSERT INTO authors (normalized_name, first_name, last_name, date_created)
                           VALUES (@NormalizedName, @FirstName, @LastName, @DateCreated)
                           RETURNING id
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        return await connection.QuerySingleAsync<int>(sql, 
            new { 
                author.NormalizedName,
                author.FirstName,
                author.LastName,
                DateCreated = DateTimeOffset.UtcNow 
            });
    }

    public async Task<Author?> GetAuthorByNameAsync(string name)
    {
        const string sql = """
                           SELECT * FROM authors 
                           WHERE normalized_name = @Name
                           LIMIT 1
                           """;
            
        var connection = await _dbContext.GetConnectionAsync();
        
        return await connection.QuerySingleOrDefaultAsync<Author>(sql, new { Name = name });
    }
}