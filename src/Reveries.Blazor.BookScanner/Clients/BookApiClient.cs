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
        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("ISBN is required.");

        var response = await SendAsync(() => _httpClient.GetAsync($"books/isbn/{isbn}"));

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<BookDetailsDto>();

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        throw new ApiException(error?.Message ?? "Unknown error", response.StatusCode);
    }

    public async Task<bool> ExistsAsync(string isbn)
    {
        var response = await SendAsync(() => _httpClient.GetAsync($"books/{isbn}/exists"));
        return response.IsSuccessStatusCode && await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task<Guid> CreateAsync(BookDetailsDto book)
    {
        var request = MapToRequest(book);
        var response = await SendAsync(() => _httpClient.PostAsJsonAsync("books", request));

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<Guid>();

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        throw new ApiException(error?.Message ?? "Unknown API error", response.StatusCode);
    }

    private static async Task<HttpResponseMessage> SendAsync(Func<Task<HttpResponseMessage>> request)
    {
        try
        {
            return await request();
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

    private static CreateBookRequest MapToRequest(BookDetailsDto book) => new()
    {
        Isbn13 = book.Isbn13,
        Isbn10 = book.Isbn10,
        Title = book.Title,
        Authors = book.Authors,
        Publisher = book.Publisher,
        Pages = book.Pages,
        Language = book.Language,
        PublicationDate = book.PublicationDate,
        Synopsis = book.Synopsis,
        ImageUrl = book.CoverImageUrl,
        ImageThumbnail = book.ImageThumbnailUrl,
        Msrp = book.Msrp,
        Binding = book.Binding,
        Edition = book.Edition,
        Genres = book.Genres,
        Series = book.Series,
        NumberInSeries = book.NumberInSeries,
        DeweyDecimals = book.DeweyDecimals,
        HeightCm = book.HeightCm,
        WidthCm = book.WidthCm,
        ThicknessCm = book.ThicknessCm,
        WeightG = book.WeightG,
        DataSource = book.DataSource
    };
}