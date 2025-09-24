using Reveries.Application.Common.Exceptions;
using Reveries.Application.Interfaces.Cache;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Persistence;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Entities;

namespace Reveries.Application.Services;

public class BookManagementService : IBookManagementService
{
    private readonly IIsbndbAuthorService _authorService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookCacheService _bookCacheService;

    public BookManagementService(IIsbndbAuthorService authorService, IUnitOfWork unitOfWork, IBookCacheService bookCacheService)
    {
        _authorService = authorService;
        _unitOfWork = unitOfWork;
        _bookCacheService = bookCacheService;
    }
    
    public async Task<int> CreateBookWithRelationsAsync(Book book, CancellationToken cancellationToken = default)
    {
        await ValidateBookNotExistsAsync(book);
        
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            
            await HandlePublisherAsync(book);
            await HandleAuthorsAsync(book);
            await HandleSubjectsAsync(book);
            await HandleSeriesAsync(book);
            
            var savedBookId = await _unitOfWork.Books.CreateAsync(book);
            
            if (book.Authors.Count != 0)
                await SaveBookAuthorsAsync(savedBookId, book.Authors);
            
            if (book.Subjects != null && book.Subjects.Count != 0)
                await SaveBookSubjectsAsync(savedBookId, book.Subjects);
            
            if (book.Dimensions != null)
                await SaveBookDimensionsAsync(savedBookId, book.Dimensions);
            
            if (book.DeweyDecimals.Count != 0)
                await SaveDeweyDecimalsAsync(savedBookId, book.DeweyDecimals);
            
            await _unitOfWork.CommitAsync();
            await _bookCacheService.SetBookByIsbnAsync(book, cancellationToken);

            return savedBookId;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateBooksAsync(List<Book> books, CancellationToken cancellationToken = default)
    {
        foreach (var book in books)
        {
            await _unitOfWork.Books.UpdateBookAsync(book);
            await _bookCacheService.RemoveBookByIsbnAsync(book.Isbn13 ?? book.Isbn10, cancellationToken);
        }
    }

    private async Task ValidateBookNotExistsAsync(Book book)
    {
        var existingBook = await _unitOfWork.Books.GetBookByIsbnAsync(book.Isbn13, book.Isbn10);
        if (existingBook != null)
        {
            var isbnUsed = existingBook.Isbn13 == book.Isbn13 ? book.Isbn13 : book.Isbn10;
            throw new BookAlreadyExistsException($"A book with ISBN {isbnUsed} already exists in the database.");
        }
    }

    private async Task HandleAuthorsAsync(Book book)
    {
        foreach (var author in book.Authors)
        {
            var existingAuthor = await _unitOfWork.Authors.GetAuthorByNameAsync(author.NormalizedName);
        
            if (existingAuthor != null)
            {
                author.Id = existingAuthor.Id;
            }
            else
            {
                await EnrichAuthorWithNameVariantsAsync(author);
                
                author.Id = await _unitOfWork.Authors.CreateAuthorAsync(author);
            }
        }
    }

    private async Task EnrichAuthorWithNameVariantsAsync(Author author)
    {
        var authorList = await _authorService.GetAuthorsByNameAsync(author.NormalizedName);
    
        author.NameVariants = new List<AuthorNameVariant>
        {
            new()
            {
                NameVariant = author.NormalizedName,
                IsPrimary = true
            }
        };
    
        foreach(var authorVariant in authorList.Where(v => v != author))
        {
            if(!author.NameVariants.Any(v => v.NameVariant.Equals(authorVariant.NormalizedName, StringComparison.OrdinalIgnoreCase)))
            {
                author.NameVariants.Add(new AuthorNameVariant 
                { 
                    NameVariant = authorVariant.NormalizedName,
                    IsPrimary = false
                });
            }
        }
    }

    private async Task HandlePublisherAsync(Book book)
    {
        if (book.Publisher?.Name != null)
        {
            var publisherList = await _unitOfWork.Publishers.GetPublishersByNameAsync(book.Publisher.Name);
            var existingPublisher = publisherList.FirstOrDefault();
            
            if (existingPublisher == null)
            {
                await _unitOfWork.Publishers.CreatePublisherAsync(book.Publisher);
            }
            else
            {
                book.Publisher.Id = existingPublisher.Id;
            }
        }
    }
    
    private async Task HandleSubjectsAsync(Book book)
    {
        if (book.Subjects != null)
        {
            foreach (var subject in book.Subjects)
            {
                var existingSubject = await _unitOfWork.Subjects.GetSubjectByNameAsync(subject.Genre);
                if (existingSubject == null)
                {
                    await _unitOfWork.Subjects.CreateSubjectAsync(subject);
                }
                else
                {
                    subject.Id = existingSubject.Id;
                }
            }
        }
    }
    
    private async Task HandleSeriesAsync(Book book)
    {
        if (book.Series != null)
        {
            var existingSeries = await _unitOfWork.Series.GetSeriesByNameAsync(book.Series.Name);
            if (existingSeries == null)
            {
                await _unitOfWork.Series.CreateSeriesAsync(book.Series);
            }
            else
            {
                book.Series.Id = existingSeries.Id;
            }
        }
    }
    
    private async Task SaveBookAuthorsAsync(int bookId, IEnumerable<Author> authors)
    {
        await _unitOfWork.BookAuthors.SaveBookAuthorsAsync(bookId, authors);
    }
    
    private async Task SaveBookSubjectsAsync(int bookId, IEnumerable<Subject> subjects)
    {
        await _unitOfWork.BookSubjects.SaveBookSubjectsAsync(bookId, subjects);
    }
    
    private async Task SaveBookDimensionsAsync(int bookId, BookDimensions? dimensions)
    {
        if (dimensions != null)
        {
            await _unitOfWork.BookDimensions.SaveBookDimensionsAsync(bookId, dimensions);
        }
    }

    private async Task SaveDeweyDecimalsAsync(int bookId, IEnumerable<DeweyDecimal>? deweyDecimals)
    {
        var decimalsList = deweyDecimals?.ToList();
        if (decimalsList == null || decimalsList.Count == 0)
            return;

        await _unitOfWork.DeweyDecimals.SaveDeweyDecimalsAsync(bookId, decimalsList);
    }
}