using Dapper;
using Reveries.Application.Interfaces.Persistence;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces.Persistence.Repositories;
using Reveries.Infrastructure.Postgresql.DTOs;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly IDbContext _dbContext;
    
    public AuthorRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<int> CreateAuthorAsync(Author author)
    {
        const string authorSql = """
                                 INSERT INTO authors (normalized_name, first_name, last_name)
                                 VALUES (@NormalizedName, @FirstName, @LastName)
                                 RETURNING id
                                 """;

        const string variantSql = """
                                  INSERT INTO author_name_variants (author_id, name_variant, is_primary)
                                  VALUES (@AuthorId, @NameVariant, @IsPrimary)
                                  """;

        var connection = await _dbContext.GetConnectionAsync();

        var authorDto = author.ToDto();
        
        // Insert the author first
        var authorId = await connection.QuerySingleAsync<int>(authorSql, authorDto);
        
        author.Id = authorId;

        // If there are name variants, insert them
        if (author.NameVariants.Count > 0)
        {
            var variantDtos = author.NameVariants.Select(variant => new
            {
                AuthorId = authorId,
                variant.NameVariant,
                variant.IsPrimary
            }).ToList();

            await connection.ExecuteAsync(variantSql, variantDtos);
        }
    
        return authorId;
    }
    
    public async Task<Author?> GetAuthorByNameAsync(string name)
    {
        const string sql = """
                           SELECT a.id,
                                  a.normalized_name,
                                  a.first_name,
                                  a.last_name,
                                  a.date_created
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
                           SELECT a.id as AuthorId,
                                  a.normalized_name,
                                  a.first_name,
                                  a.last_name,
                                  a.date_created as DateCreatedAuthor
                           FROM authors a
                           WHERE a.first_name ILIKE @Pattern
                              OR a.last_name  ILIKE @Pattern
                              OR a.normalized_name ILIKE @Pattern
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        var authorDtos = await connection.QueryAsync<AuthorDto>(
            sql,
            new { Pattern = $"%{name}%" });

        return authorDtos.Select(dto => dto.ToDomain()).ToList();
    }

}