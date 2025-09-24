using Reveries.Application.Interfaces.Persistence.Repositories;

namespace Reveries.Application.Interfaces.Persistence;

public interface IUnitOfWork
{
    IBookRepository Books { get; }
    IAuthorRepository Authors { get; }
    ISeriesRepository Series { get; }
    IBookDimensionsRepository BookDimensions { get; }
    IBookSubjectsRepository BookSubjects { get; }
    IDeweyDecimalRepository DeweyDecimals { get; }
    IPublisherRepository Publishers { get; }
    IBookAuthorsRepository BookAuthors { get; }
    ISubjectRepository Subjects { get; }
    
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
