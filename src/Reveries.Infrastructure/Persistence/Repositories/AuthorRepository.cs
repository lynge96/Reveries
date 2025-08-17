using Dapper;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces;
using Reveries.Core.Interfaces.Repositories;
using Reveries.Infrastructure.Interfaces.Persistence;
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
        const string authorSql = """
                                 INSERT INTO authors (normalized_name, first_name, last_name, date_created)
                                 VALUES (@NormalizedName, @FirstName, @LastName, @DateCreated)
                                 RETURNING id
                                 """;

        const string variantSql = """
                                  INSERT INTO author_name_variants (author_id, name_variant, is_primary, date_created)
                                  VALUES (@AuthorId, @NameVariant, @IsPrimary, @DateCreated)
                                  """;

        var connection = await _dbContext.GetConnectionAsync();
    
        // Insert the author first
        var authorId = await connection.QuerySingleAsync<int>(authorSql, 
            new { 
                author.NormalizedName,
                author.FirstName,
                author.LastName,
                DateCreated = DateTimeOffset.UtcNow 
            });
        
        author.Id = authorId;

        // If there are name variants, insert them
        if (author.NameVariants.Count > 0)
        {
            foreach (var variant in author.NameVariants)
            {
                variant.AuthorId = authorId;
                await connection.ExecuteAsync(variantSql, new
                {
                    AuthorId = authorId,
                    variant.NameVariant,
                    variant.IsPrimary,
                    DateCreated = DateTimeOffset.UtcNow
                });
            }
        }
    
        return authorId;
    }
    
    public async Task<Author?> GetAuthorByNameAsync(string name)
    {
        const string sql = """
                           WITH direct_match AS (
                               SELECT * FROM authors 
                               WHERE normalized_name = @Name
                               LIMIT 1
                           ),
                           variant_match AS (
                               SELECT a.* 
                               FROM authors a
                               JOIN author_name_variants anv ON a.id = anv.author_id
                               WHERE anv.name_variant = @Name
                               LIMIT 1
                           )
                           SELECT * FROM direct_match
                           UNION ALL
                           SELECT * FROM variant_match
                           WHERE NOT EXISTS (SELECT 1 FROM direct_match)
                           LIMIT 1
                           """;
            
        var connection = await _dbContext.GetConnectionAsync();
        
        return await connection.QuerySingleOrDefaultAsync<Author>(sql, new { Name = name });
    }
    
}