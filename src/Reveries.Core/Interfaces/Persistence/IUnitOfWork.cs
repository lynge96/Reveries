using Reveries.Core.Interfaces.Persistence.Repositories;

namespace Reveries.Core.Interfaces.Persistence;

public interface IUnitOfWork
{
    IBookRepository Books { get; }
    IAuthorRepository Authors { get; }
    ISeriesRepository Series { get; }
    IBookGenresRepository BookGenres { get; }
    IDeweyDecimalRepository DeweyDecimals { get; }
    IPublisherRepository Publishers { get; }
    IBookAuthorsRepository BookAuthors { get; }
    IGenreRepository Genres { get; }
    
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
