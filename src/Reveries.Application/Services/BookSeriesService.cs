using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Interfaces;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Services;

public class BookSeriesService : IBookSeriesService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public BookSeriesService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<int> SetSeriesAsync(Isbn? isbn, Series series, int? numberInSeries)
    {
        await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            var existingBook = await _unitOfWork.Books.GetBookByIsbnAsync(isbn);
            if (existingBook == null)
                throw new NotFoundException($"Book with ISBN '{isbn}' was not found.");
            
            var existingSeries = await _unitOfWork.Series.GetByNameAsync(series.Name);
            
            if (existingSeries != null)
            {
                existingBook.Book.SetSeries(existingSeries.Series, numberInSeries);
                await _unitOfWork.Books.UpdateBookSeriesAsync(existingBook, existingSeries.DbId);
            }
            else
            {
                existingBook.Book.SetSeries(series, numberInSeries);
                var newSeriesId = await _unitOfWork.Series.AddAsync(series);
                await _unitOfWork.Books.UpdateBookSeriesAsync(existingBook, newSeriesId);
            }
            
            await _unitOfWork.CommitAsync();

            return existingBook.DbId;
        }
        catch 
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}