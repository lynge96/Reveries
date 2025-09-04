using Reveries.Application.Common.Validation;
using Reveries.Application.Extensions;
using Reveries.Application.Extensions.Mappers;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.Entities;
using Reveries.Core.Enums;
using Reveries.Core.Interfaces.Persistence;

namespace Reveries.Application.Services.Isbndb;

public class IsbndbBookService : IIsbndbBookService
{
    private readonly IIsbndbBookClient _bookClient;
    private readonly IUnitOfWork _unitOfWork;

    public IsbndbBookService(IIsbndbBookClient bookClient, IUnitOfWork unitOfWork)
    {
        _bookClient = bookClient;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<List<Book>> GetBooksByIsbnStringAsync(List<string> isbnString, CancellationToken cancellationToken = default)
    {
        var booksInDb = await _unitOfWork.Books.GetBooksWithDetailsByIsbnAsync(isbnString);
        
        var foundIsbns = booksInDb.Select(b => b.Isbn13 ?? b.Isbn10).Where(i => i != null).ToHashSet();
        var missingIsbns = isbnString.Where(i => !foundIsbns.Contains(i)).ToList();

        List<Book> booksFromApi = new();

        switch (missingIsbns.Count)
        {
            case 1:
            {
                var book = await GetBookByIsbnAsync(missingIsbns[0], cancellationToken);
                if (book != null)
                    booksFromApi.Add(book);
                break;
            }
            case > 1:
                booksFromApi = await GetBooksByIsbnsAsync(missingIsbns, cancellationToken);
                break;
        }

        return booksInDb.Concat(booksFromApi).ToList();
    }
    
    public async Task<List<Book>> GetBooksByTitleAsync(List<string>? titles, string? languageCode, BookFormat format, CancellationToken cancellationToken = default)
    {
        if (titles == null || titles.Count == 0)
            return new List<Book>();
        
        var booksInDb = await _unitOfWork.Books.GetBooksWithDetailsByTitlesAsync(titles);
        
        var missingTitles = titles
            .Where(t => !booksInDb.Any(b => 
                b.Title.Contains(t, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var booksFromApi = new List<Book>();

        if (missingTitles.Count != 0)
        {
            foreach (var missingTitle in missingTitles)
            {
                var response = await _bookClient.GetBooksByQueryAsync(missingTitle, languageCode, shouldMatchAll: true, cancellationToken);
                if (response?.Books != null)
                {
                    booksFromApi.AddRange(response.Books.Select(b => b.ToBook()));
                }
            }
        }
        
        return booksInDb
            .Concat(booksFromApi)
            .FilterByFormat(format)
            .ToList();
    }

    private async Task<Book?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var normalizedIsbn = IsbnValidationHelper.ValidateSingleIsbn(isbn);

        var response = await _bookClient.GetBookByIsbnAsync(normalizedIsbn, cancellationToken);

        var bookDto = response?.Book;
        
        var book = bookDto?.ToBook();
        
        return book;
    }
    
    private async Task<List<Book>> GetBooksByIsbnsAsync(List<string> isbns, CancellationToken cancellationToken = default)
    {
        if (isbns.Count > 100)
            throw new ArgumentException("Too many ISBN numbers. Maximum is 100.");
        
        var validatedIsbns = IsbnValidationHelper.ValidateIsbns(isbns).ValidIsbns;

        var response = await _bookClient.GetBooksByIsbnsAsync(validatedIsbns, cancellationToken);
        
        if (response is null)
            return new List<Book>();
        
        return response.Data
            .Select(bookDto => bookDto.ToBook())
            .ToList();
    }
}