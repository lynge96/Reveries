using Reveries.Core.Interfaces.IRepository;

namespace Reveries.Core.Interfaces;

public interface IUnitOfWork
{
    IBookRepository Books { get; }
    IAuthorRepository Authors { get; }
    ISeriesRepository Series { get; }
    IDeweyDecimalsRepository DeweyDecimalses { get; }
    IPublisherRepository Publishers { get; }
    IGenreRepository Genres { get; }
    
    IBookAuthorsRepository BookAuthors { get; }
    IBookGenresRepository BookGenres { get; }
    IBookDeweyDecimalsRepository BookDeweyDecimals { get; }
    
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
