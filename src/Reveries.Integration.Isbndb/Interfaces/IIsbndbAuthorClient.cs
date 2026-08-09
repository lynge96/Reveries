using Reveries.Core.Models;
using Reveries.Integration.Isbndb.DTOs.Authors;

namespace Reveries.Integration.Isbndb.Interfaces;

/// <summary>
/// Provides methods to interact with the ISBNdb API for author-related data.
/// This includes searching for authors by name and retrieving books written by a specific author.
/// The interface abstracts the external API calls and returns strongly-typed DTOs.
/// </summary>
public interface IIsbndbAuthorClient
{
    Task<AuthorSearchResponseDto?> SearchAuthorsByNameAsync(Author author, CancellationToken ct = default);
    Task<AuthorBooksResponseDto?> FetchBooksByAuthorAsync(Author author, CancellationToken ct = default);
}