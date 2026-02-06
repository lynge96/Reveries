using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Interfaces;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Services;

public class BookPersistenceService : IBookPersistenceService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public BookPersistenceService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> SaveBookWithRelationsAsync(Book book)
    {
        await _unitOfWork.BeginTransactionAsync();
        
        await ValidateBookNotExistsAsync(book);
        
        try
        {

            var publisherId = await HandlePublisherAsync(book.Publisher);
            var seriesId = await HandleSeriesAsync(book.Series);
            await HandleGenresAsync(book.Genres);
            await HandleAuthorsAsync(book.Authors);
            
            var bookDbId = await _unitOfWork.Books.AddAsync(book);

            if (book.DeweyDecimals != null)
                await SaveDeweyDecimalsAsync(bookDbId, book.DeweyDecimals);

            if (book.Authors != null)
                await SaveBookAuthorsAsync(bookDbId, book.Authors);
            
            if (book.Genres != null)
                await SaveBookGenresAsync(bookDbId, book.Genres);
            
            await _unitOfWork.CommitAsync();

            return bookDbId;
        }
        catch (BookAlreadyExistsException)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    private async Task ValidateBookNotExistsAsync(Book book)
    {
        var existingBook = await _unitOfWork.Books.GetBookByIsbnAsync(book.Isbn13?.Value, book.Isbn10?.Value);
        
        if (existingBook != null)
        {
            var isbnUsed = existingBook.Isbn13 == book.Isbn13 ? book.Isbn13 : book.Isbn10;
            throw new BookAlreadyExistsException(isbnUsed!);
        }
    }
    
    private async Task<int?> HandlePublisherAsync(Publisher? publisher)
    {
        if (publisher?.Name == null)
            return null;

        var result = await _unitOfWork.Publishers.GetByNameAsync(publisher.Name);

        if (result != null)
            return result.DbId;

        var createdPublisherId = await _unitOfWork.Publishers.AddAsync(publisher);
        return createdPublisherId;
    }
    
    private async Task<int?> HandleSeriesAsync(Series? series)
    {
        if (series?.Name == null)
            return null;
        
        var result = await _unitOfWork.Series.GetByNameAsync(series.Name);
        
        if (result != null)
            return result.DbId;
        
        var createdSeriesId = await _unitOfWork.Series.AddAsync(series);
        return createdSeriesId;
    }

    private async Task HandleGenresAsync(IReadOnlyList<Genre> genres)
    {
        foreach (var genre in genres)
        {
            Genre? existingGenre = await _unitOfWork.Genres.GetByNameAsync(genre.Value);
            
            if (existingGenre != null)
            {
                genre.GenreId = existingGenre.GenreId;
            }
            else
            {
                var createdGenreId = await _unitOfWork.Genres.AddAsync(genre);
                genre.GenreId = createdGenreId;
            }
        }
    }

    private async Task HandleAuthorsAsync(IReadOnlyList<Author> authors)
    {
        foreach (var author in authors)
        {
            Author? existingAuthor = await _unitOfWork.Authors.GetByNameAsync(author.NormalizedName);
        
            if (existingAuthor != null)
            {
                author.AuthorId = existingAuthor.AuthorId;
            }
            else
            {
                var createdAuthorId = await _unitOfWork.Authors.AddAsync(author);
                author.AuthorId = createdAuthorId;
            }
        }
    }
    
    private async Task SaveDeweyDecimalsAsync(int bookId, IEnumerable<DeweyDecimal>? deweyDecimals)
    {
        var decimalsList = deweyDecimals?.ToList();
        if (decimalsList == null || decimalsList.Count == 0)
            return;

        await _unitOfWork.DeweyDecimals.SaveDeweyDecimalsAsync(bookId, decimalsList);
    }
    
    private async Task SaveBookAuthorsAsync(int bookId, IEnumerable<Author> authors)
    {
        await _unitOfWork.BookAuthors.SaveBookAuthorsAsync(bookId, authors);
    }
    
    private async Task SaveBookGenresAsync(int bookId, IEnumerable<Genre> subjects)
    {
        await _unitOfWork.BookGenres.SaveBookGenresAsync(bookId, subjects);
    }
}