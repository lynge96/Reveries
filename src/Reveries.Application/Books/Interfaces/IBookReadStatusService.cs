using Reveries.Core.Models;

namespace Reveries.Application.Books.Interfaces;

public interface IBookReadStatusService
{
    Task UpdateReadStatusAsync(Book book, CancellationToken ct = default);
}