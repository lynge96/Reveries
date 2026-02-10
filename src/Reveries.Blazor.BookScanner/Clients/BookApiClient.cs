using System.Net.Http.Json;
using Reveries.Blazor.BookScanner.Exceptions;
using Reveries.Contracts.Books;

namespace Reveries.Blazor.BookScanner.Clients;

public class BookApiClient
{
    private readonly HttpClient _httpClient;

    public BookApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<BookDetailsDto?> GetAsync(string isbn)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(isbn))
                throw new ArgumentException("ISBN is required.");
        
            var response = await _httpClient.GetAsync($"books/{isbn}");

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<BookDetailsDto>();

            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new ApiException(error?.Message ?? "Unknown error", response.StatusCode);
        }
        catch (HttpRequestException ex) when (ex.StatusCode is null)
        {
            throw new ApiConnectionException("Could not establish a connection to the API.", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new ApiConnectionException("The API request timed out.", ex);
        }
    }

    public async Task<int> CreateAsync(BookDetailsDto bookDetailsReadModel)
    {
        try
        {
            var createBookRequest = new CreateBookRequest
            {
                Isbn13 = bookDetailsReadModel.Isbn13,
                Isbn10 = bookDetailsReadModel.Isbn10,
                Title = bookDetailsReadModel.Title,
                Authors = bookDetailsReadModel.Authors,
                Publisher = bookDetailsReadModel.Publisher,
                Pages = bookDetailsReadModel.Pages,
                Language = bookDetailsReadModel.Language,
                PublicationDate = bookDetailsReadModel.PublicationDate,
                Synopsis = bookDetailsReadModel.Synopsis,
                ImageUrl = bookDetailsReadModel.CoverImageUrl,
                ImageThumbnail = bookDetailsReadModel.ImageThumbnailUrl,
                Msrp = bookDetailsReadModel.Msrp,
                Binding = bookDetailsReadModel.Binding,
                Edition = bookDetailsReadModel.Edition,
                Genres = bookDetailsReadModel.Genres,
                Series = bookDetailsReadModel.Series,
                NumberInSeries = bookDetailsReadModel.NumberInSeries,
                DeweyDecimals = bookDetailsReadModel.DeweyDecimals,
                HeightCm = bookDetailsReadModel.HeightCm,
                WidthCm = bookDetailsReadModel.WidthCm,
                ThicknessCm = bookDetailsReadModel.ThicknessCm,
                WeightG = bookDetailsReadModel.WeightG,
                DataSource = bookDetailsReadModel.DataSource
            };
            
            var response = await _httpClient.PostAsJsonAsync("books", createBookRequest);

            if (response.IsSuccessStatusCode)
            {
                var bookId = await response.Content.ReadFromJsonAsync<int>();
                return bookId;
            }

            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new ApiException(error?.Message ?? "Unknown API error", response.StatusCode);
        }
        catch (HttpRequestException ex) when (ex.StatusCode is null)
        {
            throw new ApiConnectionException("Could not establish a connection to the API.", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new ApiConnectionException("The API request timed out.", ex);
        }
    }
    
}