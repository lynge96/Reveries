using Reveries.Core.Interfaces;
using Reveries.Core.Interfaces.Persistence;
using Reveries.Core.Interfaces.Repositories;
using Reveries.Infrastructure.Interfaces.Persistence;
using Reveries.Infrastructure.Persistence.Context;

namespace Reveries.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly IPostgresDbContext _dbContext;
    private bool _disposed;
    public IBookRepository Books { get; }
    public IAuthorRepository Authors { get; }
    public IPublisherRepository Publishers { get; }
    public ISubjectRepository Subjects { get; }
    public IBookAuthorsRepository BookAuthors { get; }
    public IBookSubjectsRepository BookSubjects { get; }
    public IBookDimensionsRepository BookDimensions { get; }
    public IDeweyDecimalRepository DeweyDecimals { get; }
    
    public ISeriesRepository Series { get; }

    public UnitOfWork(IPostgresDbContext dbContext,
        IBookRepository bookRepository, 
        IAuthorRepository authorRepository, 
        IPublisherRepository publisherRepository, 
        ISubjectRepository subjectRepository,
        IBookAuthorsRepository bookAuthors,
        IBookSubjectsRepository bookSubjects,
        IBookDimensionsRepository bookDimensions,
        IDeweyDecimalRepository deweyDecimals,
        ISeriesRepository series)
    {
        _dbContext = dbContext;
        Books = bookRepository;
        Authors = authorRepository;
        Publishers = publisherRepository;
        Subjects = subjectRepository;
        BookAuthors = bookAuthors;
        BookSubjects = bookSubjects;
        BookDimensions = bookDimensions;
        DeweyDecimals = deweyDecimals;
        Series = series;
    }
    
    public async Task BeginTransactionAsync()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(UnitOfWork));
        }
        await _dbContext.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(UnitOfWork));
        }
        await _dbContext.CommitTransactionAsync();
    }

    public async Task RollbackAsync()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(UnitOfWork));
        }
        await _dbContext.RollbackTransactionAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        await _dbContext.DisposeAsync();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

}