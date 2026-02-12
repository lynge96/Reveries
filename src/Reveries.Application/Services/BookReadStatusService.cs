using Reveries.Application.Interfaces.Cache;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Interfaces;
using Reveries.Core.Models;

namespace Reveries.Application.Services;

public class BookReadStatusService : IBookReadStatusService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookCacheService _cache;
    
    public BookReadStatusService(
        IUnitOfWork unitOfWork,
        IBookCacheService cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    
    public async Task UpdateReadStatusAsync(Book book, CancellationToken ct)
    {
        await _unitOfWork.Books.UpdateBookReadStatusAsync(book);
        await _cache.RemoveBookByIsbnAsync(book.Isbn13 ?? book.Isbn10, ct);
    }
}