using Reveries.Application.Interfaces.Services;
using Reveries.Core.Interfaces;
using Reveries.Core.Models;

namespace Reveries.Application.Services;

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
        
        try
        {
            if (book.Series?.Name == null) return -1;
            
            var existingSeries = await _unitOfWork.Series.GetByNameAsync(book.Series.Name);
            int seriesId;
            
            if (book.Series != null)
            {

            }
            
            
            
        }
        catch 
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}