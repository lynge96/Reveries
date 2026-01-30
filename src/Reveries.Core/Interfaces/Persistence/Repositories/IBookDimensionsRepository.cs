using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Core.Interfaces.Persistence.Repositories;

public interface IBookDimensionsRepository
{
    Task SaveBookDimensionsAsync(int? bookId, BookDimensions dimensions);
}
