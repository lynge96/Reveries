using Reveries.Application.Interfaces.GoogleBooks;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces.Persistence;

namespace Reveries.Application.Services;

public class BookEnrichmentService : IBookEnrichmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIsbndbBookService _isbndbService;
    private readonly IGoogleBookService _googleService;

    public BookEnrichmentService(IUnitOfWork unitOfWork, IIsbndbBookService isbndbBookService, IGoogleBookService googleBookService)
    {
        _unitOfWork = unitOfWork;
        _isbndbService = isbndbBookService;
        _googleService = googleBookService;
    }

    public async Task<Book?> EnrichBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var result = await EnrichBooksByIsbnsAsync(new List<string> { isbn }, cancellationToken);
        
        return result.FirstOrDefault();
    }

    public async Task<List<Book>> EnrichBooksByIsbnsAsync(List<string> isbns, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Book>> SearchBooksByTitleAsync(string title, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}