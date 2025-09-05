using Reveries.Application.Extensions.Mappers;
using Reveries.Application.Interfaces.GoogleBooks;
using Reveries.Core.Entities;

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

        var googleVolume = response.Items.First().VolumeInfo;

        return googleVolume.ToBook();
    }
}