using System.Net.Http.Json;
using Reveries.Blazor.BookScanner.Exceptions;
using Reveries.Contracts.Books;
using Reveries.Contracts.DTOs;

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

    public async Task<int> CreateAsync(BookDetailsDto bookDetailsDto)
    {
        try
        {
            var createBookRequest = new CreateBookRequest
            {
                Isbn13 = bookDetailsDto.Isbn13,
                Isbn10 = bookDetailsDto.Isbn10,
                Title = bookDetailsDto.Title,
                Authors = bookDetailsDto.Authors,
                Publisher = bookDetailsDto.Publisher,
                Pages = bookDetailsDto.Pages,
                Language = bookDetailsDto.Language,
                PublicationDate = bookDetailsDto.PublicationDate,
                Synopsis = bookDetailsDto.Synopsis,
                ImageUrl = bookDetailsDto.ImageUrl,
                ImageThumbnail = bookDetailsDto.ImageThumbnail,
                Msrp = bookDetailsDto.Msrp,
                Binding = bookDetailsDto.Binding,
                Edition = bookDetailsDto.Edition,
                Genres = bookDetailsDto.Subjects,
                Series = bookDetailsDto.Series,
                NumberInSeries = bookDetailsDto.NumberInSeries,
                DeweyDecimals = bookDetailsDto.DeweyDecimal,
                HeightCm = bookDetailsDto.Dimensions?.HeightCm,
                WidthCm = bookDetailsDto.Dimensions?.WidthCm,
                ThicknessCm = bookDetailsDto.Dimensions?.ThicknessCm,
                WeightG = bookDetailsDto.Dimensions?.WeightG,
                DataSource = bookDetailsDto.DataSource
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