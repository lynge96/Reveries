using Reveries.Core.DTOs;
using Reveries.Core.DTOs.Books;

namespace Reveries.Application.Interfaces.Isbndb;

public interface IIsbndbBookClient
{
    Task<BookDetailsDto?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Returns a list of books that match the given query.
    /// </summary>
    /// <param name="query">
    /// The search string. Can be an ISBN, author name, book title, or subject.
    /// </param>
    /// <param name="languageCode">
    /// (Optional) Language filter, e.g., 'en' for English or 'da' for Danish.
    /// If null, results are returned in all languages.
    /// </param>
    /// <param name="shouldMatchAll">
    /// (Optional) Set to true to return books where the title or author exactly contains all the words in the query.
    /// Set to false for broader matching.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="BooksQueryResponseDto"/> containing matching books and the total count.
    /// </returns>
    Task<BooksQueryResponseDto?> GetBooksByQueryAsync(string query, string? languageCode, bool shouldMatchAll = false, CancellationToken cancellationToken = default);
    
    Task<BooksListResponseDto?> GetBooksByIsbnsAsync(IsbnsRequestDto isbns, CancellationToken cancellationToken = default);
}
