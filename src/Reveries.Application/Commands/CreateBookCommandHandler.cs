using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.Cache;
using Reveries.Core.Identity;
using Reveries.Core.Interfaces;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Commands;

public sealed class CreateBookCommandHandler : ICommandHandler<CreateBookCommand, BookId>
{
    private readonly IBookPersistenceService _bookPersistenceService;
    private readonly IBookCacheService _cache;
    private readonly ILogger<CreateBookCommandHandler> _logger;

    public CreateBookCommandHandler(IBookPersistenceService bookPersistenceService, IBookCacheService cache, ILogger<CreateBookCommandHandler> logger)
    {
        _bookPersistenceService = bookPersistenceService;
        _cache = cache;
        _logger = logger;
    }
    
    public async Task<BookId> Handle(CreateBookCommand command)
    {
        var book = command.Book;

        _logger.LogDebug(
            "Creating book '{Title}' with ISBN {Isbn}",
            book.Title,
            book.Isbn13?.Value ?? book.Isbn10?.Value);

        await _bookPersistenceService.SaveBookWithRelationsAsync(book);

        await _cache.SetBookByIsbnAsync(book);
        
        return book.Id;
    }
    
    public async Task<BookId> CreateBookWithRelationsAsync(Book book, CancellationToken ct)
    {
        _logger.LogDebug("Started creation of book '{Title}' with Isbn {Isbn}.", 
            book.Title, 
            book.Isbn13?.Value ?? book.Isbn10?.Value);
        
        await ValidateBookNotExistsAsync(book);
        
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            
            await HandlePublisherAsync(book);
            await HandleAuthorsAsync(book);
            await HandleGenresAsync(book);
            await HandleSeriesAsync(book);
            
            var savedBook = await _unitOfWork.Books.CreateAsync(book);
            
            if (book.Authors.Count != 0)
                await SaveBookAuthorsAsync(savedBook.Id, book.Authors);
            
            if (book.Genres.Count != 0)
                await SaveBookSubjectsAsync(savedBook.Id, book.Genres);
            
            if (book.DeweyDecimals.Count != 0)
                await SaveDeweyDecimalsAsync(savedBook.Id, book.DeweyDecimals);
            
            await _unitOfWork.CommitAsync();
            await _cache.SetBookByIsbnAsync(book, ct);

            _logger.LogInformation(
                "Book created successfully. Id={BookId}, ISBN13={Isbn13}, Authors={Authors}, Publisher={Publisher}",
                savedBook,
                book.Isbn13,
                string.Join(", ", book.Authors.Select(a => a.ToString())),
                book.Publisher?.ToString());
            
            return savedBook.Id;
        }
        catch (BookAlreadyExistsException)
        {
            _logger.LogInformation(
                "CreateBookWithRelations aborted. Book already exists. Title={Title}, ISBN13={Isbn13}, ISBN10={Isbn10}",
                book.Title,
                book.Isbn13,
                book.Isbn10);
            
            await _unitOfWork.RollbackAsync();
            throw;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateBooksAsync(List<Book> books, CancellationToken ct)
    {
        foreach (var book in books)
        {
            await _unitOfWork.Books.UpdateBookAsync(book);
            await _cache.RemoveBookByIsbnAsync(book.Isbn13 ?? book.Isbn10, ct);
        }
    }

    private async Task ValidateBookNotExistsAsync(Book book)
    {
        var existingBook = await _unitOfWork.Books.GetBookByIsbnAsync(book.Isbn13, book.Isbn10);
        
        if (existingBook != null)
        {
            var isbnUsed = existingBook.Isbn13 == book.Isbn13 ? book.Isbn13 : book.Isbn10;
            throw new BookAlreadyExistsException(isbnUsed!);
        }
    }

    private async Task HandleAuthorsAsync(Book book)
    {
        foreach (var author in book.Authors)
        {
            var existingAuthor = await _unitOfWork.Authors.GetAuthorByNameAsync(author.NormalizedName);
        
            if (existingAuthor != null)
            {
                book.AddAuthor(existingAuthor);
            }
            else
            {
                await EnrichAuthorWithNameVariantsAsync(author);
                
                var createdAuthor = await _unitOfWork.Authors.CreateAuthorAsync(author);
                book.AddAuthor(createdAuthor);
            }
        }
    }

    private async Task EnrichAuthorWithNameVariantsAsync(Author author)
    {
        var authorList = await _authorService.GetAuthorsByNameAsync(author.NormalizedName);
    
        author.AddNameVariant(author.NormalizedName, true);
    
        foreach(var authorVariant in authorList.Where(v => v != author))
        {
            if(!author.NameVariants.Any(v => v.NameVariant.Equals(authorVariant.NormalizedName, StringComparison.OrdinalIgnoreCase)))
            {
                author.AddNameVariant(authorVariant.NormalizedName, false);
            }
        }
    }
    
    
    private async Task SaveBookAuthorsAsync(int? bookId, IEnumerable<Author> authors)
    {
        await _unitOfWork.BookAuthors.SaveBookAuthorsAsync(bookId, authors);
    }
    
    private async Task SaveBookSubjectsAsync(int? bookId, IEnumerable<Genre> subjects)
    {
        await _unitOfWork.BookGenres.SaveBookGenresAsync(bookId, subjects);
    }

    private async Task SaveDeweyDecimalsAsync(int? bookId, IEnumerable<DeweyDecimal>? deweyDecimals)
    {
        var decimalsList = deweyDecimals?.ToList();
        if (decimalsList == null || decimalsList.Count == 0)
            return;

        await _unitOfWork.DeweyDecimals.SaveDeweyDecimalsAsync(bookId, decimalsList);
    }
}