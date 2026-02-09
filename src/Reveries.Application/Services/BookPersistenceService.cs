using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Interfaces;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Core.ValueObjects.DTOs;

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
            
            var genres = await HandleGenresAsync(book.Genres);
            var authors = await HandleAuthorsAsync(book.Authors);
            var deweyDecimals = await HandleDeweyDecimalsAsync(book.DeweyDecimals);
            
            var bookDbId = await _unitOfWork.Books.AddAsync(book, publisherId, seriesId);

            await SaveBookAuthorsAsync(bookDbId, authors);
            await SaveBookGenresAsync(bookDbId, genres);
            await SaveBookDeweyDecimalsAsync(bookDbId, deweyDecimals);
            
            await _unitOfWork.CommitAsync();

            return bookDbId;
        }
        catch
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
            var isbnUsed = existingBook.Book.Isbn13 == book.Isbn13 ? book.Isbn13 : book.Isbn10;
            throw new BookAlreadyExistsException(isbnUsed!);
        }
    }
    
    private async Task<int?> HandlePublisherAsync(Publisher? publisher)
    {
        if (publisher?.Name == null)
            return null;

        var existingPublisher = await _unitOfWork.Publishers.GetByNameAsync(publisher.Name);

        if (existingPublisher != null)
            return existingPublisher.DbId;

        var createdPublisherId = await _unitOfWork.Publishers.AddAsync(publisher);
        return createdPublisherId;
    }
    
    private async Task<int?> HandleSeriesAsync(Series? series)
    {
        if (series?.Name == null)
            return null;
        
        var existingSeries = await _unitOfWork.Series.GetByNameAsync(series.Name);
        
        if (existingSeries != null)
            return existingSeries.DbId;
        
        var createdSeriesId = await _unitOfWork.Series.AddAsync(series);
        return createdSeriesId;
    }

    private async Task<List<GenreWithId>> HandleGenresAsync(IReadOnlyList<Genre> genres)
    {
        var genreNames = genres.Select(g => g.Value).Distinct();
        
        var existingGenres = await _unitOfWork.Genres.GetByNamesAsync(genreNames);

        var existingByName = existingGenres.ToDictionary(g => g.Genre.Value);

        var result = new List<GenreWithId>();

        foreach (var genre in genres)
        {
            if (existingByName.TryGetValue(genre.Value, out var existing))
            {
                result.Add(existing);
            }
            else
            {
                var id = await _unitOfWork.Genres.AddAsync(genre);
                result.Add(new GenreWithId(genre, id));
            }
        }

        return result;
    }

    private async Task<List<AuthorWithId>> HandleAuthorsAsync(IReadOnlyList<Author> authors)
    {
        var authorNames = authors.Select(a => a.NormalizedName).Distinct();
        
        var existingAuthors = await _unitOfWork.Authors.GetByNamesAsync(authorNames);
        
        var existingByName = existingAuthors.ToDictionary(a => a.Author.NormalizedName);
        
        var result = new List<AuthorWithId>();

        foreach (var author in authors)
        {
            if (existingByName.TryGetValue(author.NormalizedName, out var existing))
            {
                result.Add(existing);
            }
            else
            {
                var id = await _unitOfWork.Authors.AddAsync(author);
                result.Add(new AuthorWithId(author, id));
            }       
        }
        
        return result;
    }
    
    private async Task<List<DeweyDecimalWithId>> HandleDeweyDecimalsAsync(IReadOnlyList<DeweyDecimal> deweyDecimals)
    {
        var deweyDecimalCodes = deweyDecimals.Select(d => d.Code).Distinct();
        
        var existingDeweyDecimals = await _unitOfWork.DeweyDecimals.GetByCodesAsync(deweyDecimalCodes);

        var existingByCode = existingDeweyDecimals.ToDictionary(dd => dd.DeweyDecimal.Code);
        
        var result = new List<DeweyDecimalWithId>();

        foreach (var deweyDecimal in deweyDecimals)
        {
            if (existingByCode.TryGetValue(deweyDecimal.Code, out var existing))
            {
                result.Add(existing);
            }
            else
            {
                var id = await _unitOfWork.DeweyDecimals.AddAsync(deweyDecimal);
                result.Add(new DeweyDecimalWithId(deweyDecimal, id));
            }
        }
        
        return result;
    }
    
    private async Task SaveBookAuthorsAsync(int bookId, List<AuthorWithId> authors)
    {
        await _unitOfWork.BookAuthors.AddAsync(bookId, authors);
    }
    
    private async Task SaveBookGenresAsync(int bookId, List<GenreWithId> genres)
    {
        await _unitOfWork.BookGenres.AddAsync(bookId, genres);
    }
    
    private async Task SaveBookDeweyDecimalsAsync(int bookId, List<DeweyDecimalWithId> deweyDecimals)
    {
        await _unitOfWork.BookDeweyDecimals.AddAsync(bookId, deweyDecimals);
    }
}