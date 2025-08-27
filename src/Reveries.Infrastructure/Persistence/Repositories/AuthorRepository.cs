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
        
        author.AuthorId = authorId;

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
                           SELECT a.id as author_id,
                                  a.normalized_name,
                                  a.first_name,
                                  a.last_name,
                                  a.date_created as datecreatedauthor
                           FROM authors a
                           WHERE a.normalized_name = @Name
                              OR EXISTS (
                                  SELECT 1 
                                  FROM author_name_variants anv
                                  WHERE anv.author_id = a.id 
                                    AND anv.name_variant = @Name
                              )
                           ORDER BY CASE WHEN a.normalized_name = @Name THEN 1 ELSE 2 END
                           LIMIT 1;
                           """;

        var connection = await _dbContext.GetConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<Author>(sql, new { Name = name });
    }

    public async Task<List<Author>> GetAuthorsByNameAsync(string name)
    {
        const string sql = """
                           SELECT a.id as author_id,
                                  a.normalized_name,
                                  a.first_name,
                                  a.last_name,
                                  a.date_created as datecreatedauthor
                           FROM authors a
                           WHERE a.first_name ILIKE @Pattern
                              OR a.last_name  ILIKE @Pattern
                              OR a.normalized_name ILIKE @Pattern
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        var authors = await connection.QueryAsync<Author>(
            sql,
            new { Pattern = $"%{name}%" });

        return authors.ToList();
    }

}