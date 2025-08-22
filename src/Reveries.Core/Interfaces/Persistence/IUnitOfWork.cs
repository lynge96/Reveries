using Reveries.Core.Interfaces.Repositories;

namespace Reveries.Core.Interfaces.Persistence;

public interface IUnitOfWork
{
    IBookRepository Books { get; }
    
    IAuthorRepository Authors { get; }
    
    IPublisherRepository Publishers { get; }
    
    ISubjectRepository Subjects { get; }
    
    IBookAuthorsRepository BookAuthors { get; }
    
    IBookSubjectsRepository BookSubjects { get; }

    IBookDimensionsRepository BookDimensions { get; }
    
    IDeweyDecimalRepository DeweyDecimals { get; }

    Task BeginTransactionAsync();
    
    Task CommitAsync();
    
    Task RollbackAsync();
}
