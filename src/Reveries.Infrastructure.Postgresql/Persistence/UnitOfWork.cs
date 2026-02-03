using Reveries.Application.Interfaces.Persistence;
using Reveries.Core.Interfaces.Persistence;
using Reveries.Core.Interfaces.Persistence.Repositories;

namespace Reveries.Infrastructure.Postgresql.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbContext _dbContext;
    public IBookRepository Books { get; }
    public IAuthorRepository Authors { get; }
    public ISeriesRepository Series { get; }
    public IBookDimensionsRepository BookDimensions { get; }
    public IBookGenresRepository BookGenres { get; }
    public IDeweyDecimalRepository DeweyDecimals { get; }
    public IPublisherRepository Publishers { get; }
    public IBookAuthorsRepository BookAuthors { get; }
    public IGenreRepository Genres { get; }
    
    public UnitOfWork(IDbContext dbContext,
        IBookRepository bookRepository,
        IAuthorRepository authorRepository,
        ISeriesRepository seriesRepository,
        IBookDimensionsRepository bookDimensionsRepository,
        IPublisherRepository publisherRepository,
        IBookAuthorsRepository bookAuthorsRepository,
        IBookGenresRepository bookGenresRepository,
        IDeweyDecimalRepository deweyDecimalRepository,
        IGenreRepository genreRepository)
    {
        _dbContext = dbContext;
        Books = bookRepository;
        Authors = authorRepository;
        Series = seriesRepository;
        BookDimensions = bookDimensionsRepository;
        Publishers = publisherRepository;
        BookAuthors = bookAuthorsRepository;
        BookGenres = bookGenresRepository;
        DeweyDecimals = deweyDecimalRepository;
        Genres = genreRepository;
    }
    
    public Task BeginTransactionAsync() => _dbContext.BeginTransactionAsync();
    public Task CommitAsync() => _dbContext.CommitTransactionAsync();
    public Task RollbackAsync() => _dbContext.RollbackTransactionAsync();

}