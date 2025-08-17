using Reveries.Core.Interfaces.Repositories;

namespace Reveries.Core.Interfaces.Persistence;

public interface IUnitOfWork
{
    IBookRepository Books { get; }
    
    IAuthorRepository Authors { get; }
    
    IPublisherRepository Publishers { get; }
    
    ISubjectRepository Subjects { get; }
    
    IBookAuthorsRepository BookAuthorses { get; }
    
    IBookSubjectsRepository BookSubjectses { get; }

    IBookDimensionsRepository BookDimensions { get; }

    Task BeginTransactionAsync();
    
    Task CommitAsync();
    
    Task RollbackAsync();
}
