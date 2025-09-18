using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Services;

/// <summary>
/// Provides methods for managing books and their related entities,
/// such as authors, publishers, subjects, and series.
/// </summary>
public interface IBookManagementService
{
    /// <summary>
    /// Creates a new book along with its related entities in the database.
    /// This includes handling publishers, authors, subjects, series, dimensions, and Dewey decimals.
    /// </summary>
    /// <param name="book">
    /// The <see cref="Book"/> entity to create. Must contain all relevant details for the book and its relations.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// The database ID of the newly created book.
    /// </returns>
    Task<int> CreateBookWithRelationsAsync(Book book, CancellationToken cancellationToken = default);
    
    Task UpdateBooksAsync(List<Book> books, CancellationToken cancellationToken = default);
}
