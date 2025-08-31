using Reveries.Core.Entities;

namespace Reveries.Core.Interfaces.Repositories;

public interface IBookDimensionsRepository
{
    Task SaveBookDimensionsAsync(int bookId, BookDimensions dimensions);
}
