using Reveries.Application.Extensions.Mappers;
using Reveries.Application.Interfaces.GoogleBooks;
using Reveries.Core.Entities;
using Reveries.Core.Enums;

namespace Reveries.Application.Services.GoogleBooks;

public class GoogleBookService : IGoogleBookService
{
    private readonly IGoogleBooksClient _googleBooksClient;
    
    public GoogleBookService(IGoogleBooksClient googleBooksClient)
    {
        _googleBooksClient = googleBooksClient;
    }
    
    public async Task<Book?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var response = await _googleBooksClient.GetBookByIsbnAsync(isbn, cancellationToken);
        if (response?.Items == null || response.Items.Count == 0)
            return null;

        var item = response.Items.First();

        var volumeResponse = await _googleBooksClient.GetBookByVolumeIdAsync(item.Id, cancellationToken);
        if (volumeResponse?.VolumeInfo == null)
            return item.VolumeInfo.ToBook();

        var primaryBook = item.VolumeInfo.ToBook();
        var volumeBook = volumeResponse.VolumeInfo.ToBook();

        var mergedBook = MergeGoogleBooks(primaryBook, volumeBook);
        return mergedBook;
    }

    private static Book MergeGoogleBooks(Book book, Book volume)
    {
        return new Book
        {
            DataSource = DataSource.GoogleBooksApi,
            Title = book.Title,
            Isbn13 = book.Isbn13 ?? volume.Isbn13,
            Isbn10 = book.Isbn10 ?? volume.Isbn10,
            Pages =  (book.Pages > 0) ? book.Pages : volume.Pages,
            Synopsis = volume.Synopsis ?? book.Synopsis,
            Authors = book.Authors,
            Edition = book.Edition,
            Publisher = book.Publisher ?? volume.Publisher,
            PublishDate = book.PublishDate ?? volume.PublishDate,
            Subjects = volume.Subjects,
            Language = book.Language ?? volume.Language,
            LanguageIso639 = book.LanguageIso639 ?? volume.LanguageIso639,
            Binding = book.Binding,
        };
    }
}