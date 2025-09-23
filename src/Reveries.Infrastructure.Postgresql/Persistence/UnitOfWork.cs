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
    public IBookSubjectsRepository BookSubjects { get; }
    public IDeweyDecimalRepository DeweyDecimals { get; }
    public IPublisherRepository Publishers { get; }
    public IBookAuthorsRepository BookAuthors { get; }
    public ISubjectRepository Subjects { get; }
    
    public UnitOfWork(IDbContext dbContext,
        IBookRepository bookRepository,
        IAuthorRepository authorRepository,
        ISeriesRepository seriesRepository,
        IBookDimensionsRepository bookDimensionsRepository,
        IPublisherRepository publisherRepository,
        IBookAuthorsRepository bookAuthorsRepository,
        IBookSubjectsRepository bookSubjectsRepository,
        IDeweyDecimalRepository deweyDecimalRepository,
        ISubjectRepository subjectRepository)
    {
        _dbContext = dbContext;
        Books = bookRepository;
        Authors = authorRepository;
        Series = seriesRepository;
        BookDimensions = bookDimensionsRepository;
        Publishers = publisherRepository;
        BookAuthors = bookAuthorsRepository;
        BookSubjects = bookSubjectsRepository;
        DeweyDecimals = deweyDecimalRepository;
        Subjects = subjectRepository;
    }
    
    public Task BeginTransactionAsync() => _dbContext.BeginTransactionAsync();
    public Task CommitAsync() => _dbContext.CommitTransactionAsync();
    public Task RollbackAsync() => _dbContext.RollbackTransactionAsync();

}