using Reveries.Application.Books.Interfaces;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Domain.Models;

namespace Reveries.Application.Books.Services;

public class BookReadStatusService : IBookReadStatusService
{
    private readonly IBookRepository _books;
    private readonly IBookCacheService _cache;

    public BookReadStatusService(
        IBookRepository books,
        IBookCacheService cache)
    {
        _books = books;
        _cache = cache;
    }

    public async Task UpdateReadStatusAsync(Book book, CancellationToken ct)
    {
        await _books.UpdateBookReadStatusAsync(book, ct);
        await _cache.RemoveBookByIsbnAsync(book.Isbn13 ?? book.Isbn10, ct);
    }
}