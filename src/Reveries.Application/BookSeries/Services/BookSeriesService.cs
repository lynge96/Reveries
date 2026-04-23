using Reveries.Application.Common.Abstractions;
using Reveries.Application.Common.Exceptions;
using Reveries.Core.Identity;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.BookSeries.Services;

public class BookSeriesService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public BookSeriesService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<BookId> SetSeriesAsync(Isbn? isbn, Series series, int? numberInSeries, CancellationToken ct)
    {
        await using var tx = await _unitOfWork.BeginTransactionAsync(ct);
        
        try
        {
            var existingBook = await _unitOfWork.Books.GetBookByIsbnAsync(isbn, ct: ct);
            if (existingBook == null)
                throw new NotFoundException($"Book with ISBN '{isbn}' was not found.");
            
            var existingSeries = await _unitOfWork.Series.GetByNameAsync(series.Name);
            
            if (existingSeries != null)
            {
                existingBook.SetSeries(existingSeries, numberInSeries);
                await _unitOfWork.Books.UpdateBookSeriesAsync(existingBook, existingSeries.Id.Value, ct);
            }
            else
            {
                existingBook.SetSeries(series, numberInSeries);
                var createdSeries = await _unitOfWork.Series.GetOrCreateAsync(series, ct: ct);
                await _unitOfWork.Books.UpdateBookSeriesAsync(existingBook, createdSeries!.Id.Value, ct);
            }
            
            await tx.CommitAsync(ct);

            return existingBook.Id;
        }
        catch 
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}