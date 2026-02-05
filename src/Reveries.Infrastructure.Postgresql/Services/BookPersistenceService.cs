using Reveries.Application.Exceptions;
using Reveries.Core.Interfaces;
using Reveries.Core.Models;
using Reveries.Infrastructure.Postgresql.Entities;
using Reveries.Infrastructure.Postgresql.Interfaces;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Services;

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
        
        var entityAggregate = book.ToEntityAggregate();
        
        await ValidateBookNotExistsAsync(entityAggregate.Book);
        
        try
        {
            await HandlePublisherAsync(entityAggregate);
            await HandleSeriesAsync(entityAggregate);
            await HandleGenresAsync(entityAggregate);
            await HandleAuthorsAsync(entityAggregate);
            
            var bookDbId = await _unitOfWork.Books.AddAsync(entityAggregate.Book);

            if (entityAggregate.DeweyDecimals != null)
                await SaveDeweyDecimalsAsync(bookDbId, entityAggregate.DeweyDecimals);

            if (entityAggregate.Authors != null)
                await SaveBookAuthorsAsync(bookDbId, entityAggregate.Authors);
            
            if (entityAggregate.Genres != null)
                await SaveBookSubjectsAsync(bookDbId, entityAggregate.Genres);
            
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

    private async Task ValidateBookNotExistsAsync(BookEntity book)
    {
        var existingBook = await _unitOfWork.Books.GetBookByIsbnAsync(book.Isbn13, book.Isbn10);
        
        if (existingBook != null)
        {
            var isbnUsed = existingBook.Isbn13 == book.Isbn13 ? book.Isbn13 : book.Isbn10;
            throw new BookAlreadyExistsException(isbnUsed!);
        }
    }
    
    private async Task HandlePublisherAsync(BookAggregateEntity aggregate)
    {
        var publisher = aggregate.Publisher;
        
        if (publisher != null)
        {
            PublisherEntity? existingPublisher = await _unitOfWork.Publishers.GetByNameAsync(publisher.PublisherName);

            if (existingPublisher != null)
            {
                publisher.PublisherId = existingPublisher.PublisherId;
            }
            else
            {
                var createdPublisherId = await _unitOfWork.Publishers.AddAsync(publisher);
                publisher.PublisherId = createdPublisherId;
            }
        }
    }

    private async Task HandleSeriesAsync(BookAggregateEntity aggregate)
    {
        var series = aggregate.Series;

        if (series != null)
        {
            SeriesEntity? existingSeries = await _unitOfWork.Series.GetByNameAsync(series.SeriesName);
            
            if (existingSeries != null)
            {
                series.SeriesId = existingSeries.SeriesId;
            }
            else
            {
                var createdSeriesId = await _unitOfWork.Series.AddAsync(series);
                series.SeriesId = createdSeriesId;
            }
        }
    }

    private async Task HandleGenresAsync(BookAggregateEntity aggregate)
    {
        var genres = aggregate.Genres;
        if (genres == null) return;
        
        foreach (var genre in genres)
        {
            GenreEntity? existingGenre = await _unitOfWork.Genres.GetByNameAsync(genre.Name);
            
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

    private async Task HandleAuthorsAsync(BookAggregateEntity aggregate)
    {
        var authors = aggregate.Authors;
        if (authors == null) return;
        
        foreach (var author in authors)
        {
            AuthorEntity? existingAuthor = await _unitOfWork.Authors.GetByNameAsync(author.NormalizedName);
        
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
    
    private async Task SaveDeweyDecimalsAsync(int bookId, IEnumerable<DeweyDecimalEntity>? deweyDecimals)
    {
        var decimalsList = deweyDecimals?.ToList();
        if (decimalsList == null || decimalsList.Count == 0)
            return;

        await _unitOfWork.DeweyDecimals.SaveDeweyDecimalsAsync(bookId, decimalsList);
    }
    
    private async Task SaveBookAuthorsAsync(int bookId, IEnumerable<AuthorEntity> authors)
    {
        await _unitOfWork.BookAuthors.SaveBookAuthorsAsync(bookId, authors);
    }
    
    private async Task SaveBookSubjectsAsync(int bookId, IEnumerable<GenreEntity> subjects)
    {
        await _unitOfWork.BookGenres.SaveBookGenresAsync(bookId, subjects);
    }
}