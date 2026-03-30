using Reveries.Application.Common.Exceptions;
using Reveries.Core.Interfaces;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Services.BookSeries;

public class BookSeriesService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public BookSeriesService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<int> SetSeriesAsync(Isbn? isbn, Series series, int? numberInSeries, CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            var existingBook = await _unitOfWork.Books.GetBookByIsbnAsync(isbn, ct: ct);
            if (existingBook == null)
                throw new NotFoundException($"Book with ISBN '{isbn}' was not found.");
            
            var existingSeries = await _unitOfWork.Series.GetByNameAsync(series.Name);
            
            if (existingSeries != null)
            {
                existingBook.Book.SetSeries(existingSeries.Series, numberInSeries);
                await _unitOfWork.Books.UpdateBookSeriesAsync(existingBook, existingSeries.DbId, ct);
            }
            else
            {
                existingBook.Book.SetSeries(series, numberInSeries);
                var newSeriesId = await _unitOfWork.Series.AddAsync(series);
                await _unitOfWork.Books.UpdateBookSeriesAsync(existingBook, newSeriesId, ct);
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