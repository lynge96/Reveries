using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Mediator;
using NSubstitute;
using Reveries.Api.Contracts.Books.Requests;
using Reveries.Api.Contracts.Books.Responses;
using Reveries.Application.Books.Commands.CreateBook;
using Reveries.Application.Books.Models;
using Reveries.Application.Books.Queries.FindBooksByIsbns;
using Reveries.Application.Books.Queries.GetAllBooks;
using Reveries.Application.Books.Queries.GetBookExists;
using Reveries.Domain.Editions;

namespace Reveries.Api.Tests;

public sealed class BookEndpointsTests : IDisposable
{
    private readonly BookApiFactory _factory = new();
    private IMediator Mediator => _factory.Mediator;

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetAllBooks_returns_200_with_items()
    {
        // Arrange
        IReadOnlyList<Book> books = [CreateBook()];
        Mediator.Send(Arg.Any<GetAllBooksQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<IReadOnlyList<Book>>(books));
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/books");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<BookCollectionResponse>();
        Assert.NotNull(payload);
        Assert.Single(payload!.Items);
    }

    [Fact]
    public async Task GetAllBooks_when_catalog_empty_returns_200_with_empty_list()
    {
        // Arrange
        Mediator.Send(Arg.Any<GetAllBooksQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<IReadOnlyList<Book>>([]));
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/books");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<BookCollectionResponse>();
        Assert.NotNull(payload);
        Assert.Empty(payload!.Items);
        Assert.Equal(0, payload.Count);
    }

    [Fact]
    public async Task GetBookByIsbn_with_invalid_isbn_returns_400_problem_details()
    {
        // Arrange — the endpoint builds the query (which parses the ISBN) before
        // dispatching, so an invalid ISBN throws a DomainException at the edge.
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/books/isbn/not-an-isbn");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task BookExists_returns_200_with_typed_response()
    {
        // Arrange
        Mediator.Send(Arg.Any<GetBookExistsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<bool>(true));
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/books/isbn/9780132350884/exists");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<BookExistsResponse>();
        Assert.NotNull(payload);
        Assert.True(payload!.Exists);
    }

    [Fact]
    public async Task CreateBook_returns_201_with_isbn_location_header()
    {
        // Arrange
        var editionId = EditionId.New();
        Mediator.Send(Arg.Any<CreateBookCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<EditionId>(editionId));
        var client = _factory.CreateClient();
        var request = new CreateBookRequest { Title = "Test Book", Isbn13 = "9780132350884" };

        // Act
        var response = await client.PostAsJsonAsync("/books", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/books/isbn/9780132350884", response.Headers.Location?.ToString());
        var payload = await response.Content.ReadFromJsonAsync<CreateBookResponse>();
        Assert.Equal(editionId.Value, payload?.Id);
    }

    [Fact]
    public async Task CreateBook_with_blank_title_returns_400()
    {
        // Arrange — validation short-circuits before the handler, so no mediator setup.
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/books", new CreateBookRequest { Title = "" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetBooksByIsbns_returns_200_with_items()
    {
        // Arrange
        IReadOnlyList<Book> books = [CreateBook()];
        Mediator.Send(Arg.Any<FindBooksByIsbnsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<IReadOnlyList<Book>>(books));
        var client = _factory.CreateClient();
        var request = new BookLookupRequest { Isbns = ["9780132350884"] };

        // Act
        var response = await client.PostAsJsonAsync("/books/isbns", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<BookCollectionResponse>();
        Assert.NotNull(payload);
        Assert.Single(payload!.Items);
    }

    [Fact]
    public async Task GetBooksByIsbns_with_no_isbns_returns_400()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/books/isbns", new BookLookupRequest { Isbns = [] });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Validation_problem_carries_traceId_like_other_problem_responses()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/books", new CreateBookRequest { Title = "" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        using var document = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(document);
        Assert.True(
            document!.RootElement.TryGetProperty("traceId", out _),
            "Validation ProblemDetails should include a traceId, consistent with exception-based ProblemDetails.");
    }

    private static Book CreateBook(Guid? id = null) => new()
    {
        BookId = id ?? Guid.NewGuid(),
        Title = "The Pragmatic Programmer",
        Authors = ["Andrew Hunt", "David Thomas"]
    };
}