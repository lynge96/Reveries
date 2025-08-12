namespace Reveries.Core.Interfaces;

public interface IUnitOfWork
{
    IBookRepository Books { get; }
    
    IAuthorRepository Authors { get; }
    
    IPublisherRepository Publishers { get; }
    
    ISubjectRepository Subjects { get; }
    
    Task BeginTransactionAsync();
    
    Task CommitAsync();
    
    Task RollbackAsync();
}
