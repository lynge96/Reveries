using Dapper;
using Reveries.Application.Interfaces.Persistence;
using Reveries.Core.Interfaces.Persistence.Repositories;
using Reveries.Core.Entities;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class BookDimensionsRepository : IBookDimensionsRepository
{
    private readonly IDbContext _dbContext;
    
    public BookDimensionsRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveBookDimensionsAsync(int bookId, BookDimensions dimensions)
    {
        const string sql = """
                           INSERT INTO book_dimensions (
                               book_id, height_cm, width_cm, thickness_cm, weight_g
                           ) VALUES (
                               @BookId, @HeightCm, @WidthCm, @ThicknessCm, @WeightG
                           )
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
        await connection.ExecuteAsync(sql, new
        {
            BookId = bookId,
            dimensions.HeightCm,
            dimensions.WidthCm,
            dimensions.ThicknessCm,
            dimensions.WeightG
        });
    }
}
