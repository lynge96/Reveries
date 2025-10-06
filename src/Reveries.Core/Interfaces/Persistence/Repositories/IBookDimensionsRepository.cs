using Reveries.Core.Entities;

namespace Reveries.Core.Interfaces.Persistence.Repositories;

public interface IBookDimensionsRepository
{
    Task SaveBookDimensionsAsync(int bookId, BookDimensions dimensions);
}
