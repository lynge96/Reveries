using Reveries.Application.DTOs.GoogleBooksDtos;

namespace Reveries.Application.Interfaces.GoogleBooks;

public interface IGoogleBooksClient
{
    Task<GoogleBookResponseDto?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
    Task<GoogleBookItemDto?> GetBookByVolumeIdAsync(string volumeId, CancellationToken cancellationToken = default);
    Task<GoogleBookResponseDto?> SearchBooksByTitleAsync(string title, CancellationToken cancellationToken = default);
}