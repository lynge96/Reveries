using Reveries.Core.Interfaces.IRepository;

namespace Reveries.Application.Common.Abstractions;

public interface IUnitOfWork
{
    IBookRepository Books { get; }
    IAuthorRepository Authors { get; }
    ISeriesRepository Series { get; }
    IDeweyDecimalsRepository DeweyDecimals { get; }
    IPublisherRepository Publishers { get; }
    IGenreRepository Genres { get; }
    
    IBookAuthorsRepository BookAuthors { get; }
    IBookGenresRepository BookGenres { get; }
    IBookDeweyDecimalsRepository BookDeweyDecimals { get; }
    
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
