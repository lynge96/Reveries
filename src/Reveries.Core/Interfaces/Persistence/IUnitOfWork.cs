using Reveries.Core.Interfaces.Repositories;

namespace Reveries.Core.Interfaces.Persistence;

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
