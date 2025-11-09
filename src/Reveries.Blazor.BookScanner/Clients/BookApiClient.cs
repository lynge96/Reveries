using System.Net.Http.Json;
using Reveries.Blazor.BookScanner.Exceptions;
using Reveries.Contracts.DTOs;

namespace Reveries.Blazor.BookScanner.Clients;

public class BookApiClient
{
    private readonly HttpClient _httpClient;

    public BookApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<BookDto?> GetAsync(string isbn)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(isbn))
                throw new ArgumentException("ISBN is required.");
        
            var response = await _httpClient.GetAsync($"books/{isbn}");

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<BookDto>();

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

    public async Task<int> CreateAsync(BookDto bookDto)
    {
        try
        {
            var createDto = new CreateBookDto
            {
                Isbn13 = bookDto.Isbn13,
                Isbn10 = bookDto.Isbn10,
                Title = bookDto.Title,
                Authors = bookDto.Authors,
                Publisher = bookDto.Publisher,
                Language = bookDto.Language,
                PublicationDate = bookDto.PublicationDate,
                Synopsis = bookDto.Synopsis,
                ImageUrl = bookDto.ImageUrl,
                ImageThumbnail = bookDto.ImageThumbnail,
                Msrp = bookDto.Msrp,
                Binding = bookDto.Binding,
                Edition = bookDto.Edition,
                Subjects = bookDto.Subjects,
                Series = bookDto.Series,
                NumberInSeries = bookDto.NumberInSeries,
                DeweyDecimal = bookDto.DeweyDecimal,
                DataSource = bookDto.DataSource
            };
            
            var response = await _httpClient.PostAsJsonAsync("books", createDto);

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