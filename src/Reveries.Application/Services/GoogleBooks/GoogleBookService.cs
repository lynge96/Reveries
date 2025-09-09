using Reveries.Application.Common.Mappers;
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
    
    public async Task<List<Book>> GetBooksByIsbnsAsync(List<string> isbns, CancellationToken cancellationToken = default)
    {
        var results = new List<Book>();

        foreach (var isbn in isbns)
        {
            var response = await _googleBooksClient.GetBookByIsbnAsync(isbn, cancellationToken);
            if (response?.Items == null || response.Items.Count == 0)
                continue;

            var item = response.Items.First();

            var volumeResponse = await _googleBooksClient.GetBookByVolumeIdAsync(item.Id, cancellationToken);
            if (volumeResponse?.VolumeInfo == null)
            {
                results.Add(item.VolumeInfo.ToBook());
                continue;
            }

            var primaryBook = item.VolumeInfo.ToBook();
            var volumeBook = volumeResponse.VolumeInfo.ToBook();

            results.Add(MergeGoogleBooks(primaryBook, volumeBook));
        }

        return results;
    }

    public async Task<List<Book>> GetBooksByTitleAsync(List<string> titles, CancellationToken cancellationToken = default)
    {
        var results = new List<Book>();

        foreach (var title in titles)
        {
            var response = await _googleBooksClient.SearchBooksByTitleAsync(title, cancellationToken);
            if (response?.Items == null || response.Items.Count == 0)
                continue;

            results.AddRange(response.Items.Select(i => i.VolumeInfo.ToBook()));
        } 
        
        return results;
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
            ImageThumbnail = book.ImageThumbnail ?? volume.ImageThumbnail
        };
    }
}