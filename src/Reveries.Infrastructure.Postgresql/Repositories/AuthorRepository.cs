using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Core.Models;
using Reveries.Infrastructure.Postgresql.Entities;
using Reveries.Infrastructure.Postgresql.Interfaces;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly IDbContext _dbContext;
    
    public AuthorRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Guid> AddAsync(Author author)
    {
        const string authorSql = """
                                 INSERT INTO library.authors (id, normalized_name, first_name, last_name)
                                 VALUES (@Id, @NormalizedName, @FirstName, @LastName)
                                 ON CONFLICT DO NOTHING
                                 RETURNING id
                                 """;

        const string variantSql = """
                                  INSERT INTO library.author_name_variants (author_id, name_variant, is_primary)
                                  VALUES (@AuthorId, @NameVariant, @IsPrimary)
                                  ON CONFLICT DO NOTHING
                                  """;

        var connection = await _dbContext.GetConnectionAsync();
        
        var authorEntity = author.ToDbModel();
        
        // Insert the author first
        var authorDbId = await connection.QuerySingleAsync<Guid>(authorSql, authorEntity);

        // If there are name variants, insert them
        if (authorEntity.AuthorNameVariants != null)
        {
            var variantDtos = authorEntity.AuthorNameVariants.Select(variant => new AuthorNameVariantEntity
            {
                AuthorId = authorDbId,
                NameVariant = variant.NameVariant,
                IsPrimary = variant.IsPrimary
            });

            await connection.ExecuteAsync(variantSql, variantDtos);
        }
    
        return authorDbId;
    }
    
    public async Task<Author?> GetByNameAsync(string name)
    {
        const string sql = """
                           SELECT a.id,
                                  a.normalized_name,
                                  a.first_name,
                                  a.last_name,
                                  a.date_created
                           FROM library.authors a
                           WHERE a.normalized_name = @Name
                              OR EXISTS (
                                  SELECT 1 
                                  FROM library.author_name_variants anv
                                  WHERE anv.author_id = a.id 
                                    AND anv.name_variant = @Name
                              )
                           ORDER BY CASE WHEN a.normalized_name = @Name THEN 1 ELSE 2 END
                           LIMIT 1;
                           """;

        var connection = await _dbContext.GetConnectionAsync();
        
        var authorDto = await connection.QueryFirstOrDefaultAsync<AuthorEntity>(sql, new { Name = name });

        return authorDto?.ToDomain();
    }

    public async Task<IReadOnlyList<Author>> GetByNamesAsync(IEnumerable<string> authorNames)
    {
        const string sql = """
                           SELECT DISTINCT
                               a.id,
                               a.normalized_name,
                               a.first_name,
                               a.last_name,
                               a.date_created
                           FROM library.authors a
                           LEFT JOIN library.author_name_variants anv
                               ON anv.author_id = a.id
                           WHERE
                               a.normalized_name = ANY(@Names)
                               OR anv.name_variant = ANY(@Names);
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
        
        var rows = await connection.QueryAsync<AuthorEntity>(sql, new { Names = authorNames.ToArray() });
        
        return rows.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<Author>> GetAuthorsByNameAsync(string name)
    {
        const string sql = """
                           SELECT a.id,
                                  a.normalized_name,
                                  a.first_name,
                                  a.last_name,
                                  a.date_created
                           FROM library.authors a
                           WHERE a.first_name ILIKE @Pattern
                              OR a.last_name  ILIKE @Pattern
                              OR a.normalized_name ILIKE @Pattern
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        var authorDtos = await connection.QueryAsync<AuthorEntity>(sql, new { Pattern = $"%{name}%" });

        return authorDtos.Select(a => a.ToDomain()).ToList();
    }

}