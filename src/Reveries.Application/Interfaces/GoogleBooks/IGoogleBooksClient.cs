using Reveries.Application.DTOs.GoogleBooksDtos;

namespace Reveries.Application.Interfaces.GoogleBooks;

public interface IGoogleBooksClient
{
    Task<GoogleBookResponseDto?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
    Task<GoogleBookResponseDto?> SearchBooksAsync(string query, CancellationToken cancellationToken = default);
}