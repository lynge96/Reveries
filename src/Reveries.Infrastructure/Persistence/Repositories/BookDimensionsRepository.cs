using Dapper;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces.Repositories;
using Reveries.Infrastructure.Interfaces.Persistence;

namespace Reveries.Infrastructure.Persistence.Repositories;

public class BookDimensionsRepository : IBookDimensionsRepository
{
    private readonly IPostgresDbContext _dbContext;
    
    public BookDimensionsRepository(IPostgresDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveBookDimensionsAsync(int bookId, BookDimensions dimensions)
    {
        const string sql = """
                           INSERT INTO book_dimensions (
                               book_id, height_cm, width_cm, thickness_cm, weight_g, date_created
                           ) VALUES (
                               @BookId, @HeightCm, @WidthCm, @ThicknessCm, @WeightG, @DateCreated
                           )
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
        await connection.ExecuteAsync(sql, new
        {
            BookId = bookId,
            dimensions.HeightCm,
            dimensions.WidthCm,
            dimensions.ThicknessCm,
            dimensions.WeightG,
            DateCreated = DateTimeOffset.UtcNow
        });
    }
}
