using Reveries.Application.Interfaces.Services;
using Reveries.Core.Models;
using Reveries.Infrastructure.Postgresql.Interfaces;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Services;

public class BookSeriesService : IBookSeriesService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public BookSeriesService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task<int> SetSeriesAsync(Book book)
    {
        await _unitOfWork.BeginTransactionAsync();

        var entityAggregate = book.ToEntityAggregate();
        
        try
        {
            var existingSeries = _unitOfWork.Series.GetByNameAsync(entityAggregate.Series.SeriesName);
            int seriesId;
            
            if (entityAggregate.Series != null)
            {

                seriesId = existingSeries.Id;
            }
            
            
            
        }
        catch 
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}