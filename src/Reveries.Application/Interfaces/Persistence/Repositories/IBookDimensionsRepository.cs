using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Persistence.Repositories;

public interface IBookDimensionsRepository
{
    Task SaveBookDimensionsAsync(int bookId, BookDimensions dimensions);
}
