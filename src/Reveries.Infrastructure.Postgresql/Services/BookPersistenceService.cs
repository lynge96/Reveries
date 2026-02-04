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
        
        try
        {
            // 1. Publisher
            await HandlePublisherAsync(entityAggregate);
            await HandleSeriesAsync(entityAggregate);
            await HandleGenresAsync(entityAggregate);
            await HandleAuthorsAsync(entityAggregate);

            // 2. Series
            if (book.Series != null)
            {
                var existingSeries = await _unitOfWork.Series.GetByNameAsync(book.Series.Name);
                if (existingSeries != null)
                    book.SetSeries(existingSeries);
                else
                    book.SetSeries(await _unitOfWork.Series.AddAsync(book.Series));
            }
            
            // 3. Book
            var bookDbId = await _unitOfWork.Books.AddAsync(book);
            
            // 4. Authors
            foreach (var author in book.Authors)
            {
                var authorDbId = await _unitOfWork.Authors.GetOrCreateAuthorAsync(author);
                await _unitOfWork.BookAuthors.AddAsync(bookDbId, authorDbId);
            }
            
            // 5. Genres
            foreach (var genre in book.Genres)
            {
                var genreDbId = await _unitOfWork.Genres.GetOrCreateGenreAsync(genre);
                await _unitOfWork.BookGenres.AddAsync(bookDbId, genreDbId);
            }
            
            // 6. DeweyDecimals
            if (book.DeweyDecimals.Count > 0)
                await _unitOfWork.DeweyDecimals.SaveDeweyDecimalsAsync(bookDbId, book.DeweyDecimals);

            await _unitOfWork.CommitAsync();

            return bookDbId;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
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
            var existingAuthor = await _unitOfWork.Authors.GetByNameAsync(author.NormalizedName);
        
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
    
}