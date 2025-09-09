using Reveries.Application.Common.Validation;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces.Persistence;

namespace Reveries.Application.Services;

public class BookLookupService : IBookLookupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookEnrichmentService _bookEnrichmentService;

    public BookLookupService(IUnitOfWork unitOfWork, IBookEnrichmentService bookEnrichmentService)
    {
        _unitOfWork = unitOfWork;
        _bookEnrichmentService = bookEnrichmentService;
    }
    
    public async Task<List<Book>> FindBooksByIsbnAsync(List<string> isbns, CancellationToken cancellationToken = default)
    {
        // TODO: Finder først bøger i DB, og de resterende findes igennem BookEnrichmentService, som aggregerer data fra ISBNDB og Google Books.
        var validatedIsbns = IsbnValidationHelper.ValidateIsbns(isbns);
        if (validatedIsbns.Count == 0)
            return new List<Book>();
        
        var databaseBooks = await _unitOfWork.Books.GetBooksWithDetailsByIsbnAsync(validatedIsbns);
        
        var foundIsbns = databaseBooks.Select(b => b.Isbn13 ?? b.Isbn10).Where(i => i != null).ToHashSet();
        var missingIsbns = isbns.Where(i => !foundIsbns.Contains(i)).ToList();
        
        var apiBooks = await _bookEnrichmentService.MergeBooksFromSourcesByIsbnsAsync(missingIsbns, cancellationToken);
        
        return databaseBooks.Concat(apiBooks).ToList();
    }

    public async Task<List<Book>> SearchBooksByTitleAsync(string title, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Book>> SearchBooksByAuthorAsync(string author, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}