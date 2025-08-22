using Reveries.Application.Common.Validation.Exceptions;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces.Persistence;

namespace Reveries.Application.Services;

public class BookManagementService : IBookManagementService
{
    private readonly IAuthorService _authorService;
    private readonly IUnitOfWork _unitOfWork;

    public BookManagementService(IAuthorService authorService, IUnitOfWork unitOfWork)
    {
        _authorService = authorService;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<int> SaveCompleteBookAsync(Book book, CancellationToken cancellationToken = default)
    {
        await ValidateBookNotExistsAsync(book);
        
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            
            await HandlePublisherAsync(book);
            await HandleAuthorsAsync(book);
            await HandleSubjectsAsync(book);
            
            var savedBookId = await _unitOfWork.Books.CreateBookAsync(book);
            
            if (book.Authors.Count != 0)
                await SaveBookAuthorsAsync(savedBookId, book.Authors);
            
            if (book.Subjects.Count != 0)
                await SaveBookSubjectsAsync(savedBookId, book.Subjects);
            
            if (book.Dimensions != null)
                await SaveBookDimensionsAsync(savedBookId, book.Dimensions);
            
            if (book.DeweyDecimals.Count != 0)
                await SaveDeweyDecimalsAsync(savedBookId, book.DeweyDecimals);
            
            await _unitOfWork.CommitAsync();

            return savedBookId;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
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
                author.AuthorId = existingAuthor.AuthorId;
            }
            else
            {
                await EnrichAuthorWithNameVariantsAsync(author);
                
                author.AuthorId = await _unitOfWork.Authors.CreateAuthorAsync(author);
            }
        }
    }

    private async Task EnrichAuthorWithNameVariantsAsync(Author author)
    {
        var variants = await _authorService.GetAuthorsByNameAsync(author.NormalizedName);
    
        author.NameVariants = new List<AuthorNameVariant>
        {
            new()
            {
                NameVariant = author.NormalizedName,
                IsPrimary = true
            }
        };
    
        foreach(var variant in variants.Where(v => v != author.NormalizedName))
        {
            author.NameVariants.Add(new AuthorNameVariant 
            { 
                NameVariant = variant,
                IsPrimary = false
            });
        }
    }

    private async Task HandlePublisherAsync(Book book)
    {
        if (book.Publisher != null)
        {
            var existingPublisher = await _unitOfWork.Publishers.GetPublisherByNameAsync(book.Publisher.Name);
            if (existingPublisher == null)
            {
                book.Publisher.PublisherId = await _unitOfWork.Publishers.CreatePublisherAsync(book.Publisher);
            }
            else
            {
                book.Publisher.PublisherId = existingPublisher.PublisherId;
            }
        }
    }
    
    private async Task HandleSubjectsAsync(Book book)
    {
        foreach (var subject in book.Subjects)
        {
            var existingSubject = await _unitOfWork.Subjects.GetSubjectByNameAsync(subject.Genre);
            if (existingSubject == null)
            {
                subject.SubjectId = await _unitOfWork.Subjects.CreateSubjectAsync(subject);
            }
            else
            {
                subject.SubjectId = existingSubject.SubjectId;
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