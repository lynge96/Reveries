using System.Net;
using System.Net.Http.Json;
using Mediator;
using NSubstitute;
using Reveries.Application.Books.Commands.CreateBook;
using Reveries.Application.Books.Models;
using Reveries.Application.Books.Queries.FindBookByIsbn;
using Reveries.Application.Books.Queries.GetAllBooks;
using Reveries.Application.Books.Queries.GetBookById;
using Reveries.Application.Books.Queries.GetBookExists;
using Reveries.Application.BookSeries.Commands.SetBookSeries;
using Reveries.Application.Common.Exceptions;
using Reveries.Contracts.Books.Dtos;
using Reveries.Contracts.Books.Requests;
using Reveries.Contracts.Books.Responses;
using Reveries.Domain.Editions;
using Reveries.Domain.Works;

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
        IReadOnlyList<BookDetails> books = [CreateBookDetails()];
        Mediator.Send(Arg.Any<GetAllBooksQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<IReadOnlyList<BookDetails>>(books));
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/books");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<BooksResponse>();
        Assert.NotNull(payload);
        Assert.Single(payload!.Items);
    }

    [Fact]
    public async Task GetBookById_returns_200_with_book()
    {
        // Arrange
        var id = Guid.NewGuid();
        Mediator.Send(Arg.Any<GetBookByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<BookDetails>(CreateBookDetails(id)));
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/books/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<BookDetailsDto>();
        Assert.NotNull(payload);
        Assert.Equal(id, payload!.BookId);
    }

    [Fact]
    public async Task GetBookById_when_not_found_returns_404_problem_details()
    {
        // Arrange
        Mediator.Send(Arg.Any<GetBookByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<BookDetails>(
                Task.FromException<BookDetails>(new NotFoundException("missing"))));
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/books/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await response.Content.ReadFromJsonAsync<ProblemPayload>();
        Assert.Equal((int)HttpStatusCode.NotFound, problem?.Status);
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
    public async Task BookExists_returns_200_with_boolean()
    {
        // Arrange
        Mediator.Send(Arg.Any<GetBookExistsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<bool>(true));
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/books/isbn/9780132350884/exists");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(await response.Content.ReadFromJsonAsync<bool>());
    }

    [Fact]
    public async Task CreateBook_returns_201_with_location_header()
    {
        // Arrange
        var editionId = EditionId.New();
        Mediator.Send(Arg.Any<CreateBookCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<EditionId>(editionId));
        var client = _factory.CreateClient();
        var request = new CreateBookRequest { Title = "Test Book" };

        // Act
        var response = await client.PostAsJsonAsync("/books", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"/books/{editionId.Value}", response.Headers.Location?.ToString());
        var payload = await response.Content.ReadFromJsonAsync<CreateBookResponse>();
        Assert.Equal(editionId.Value, payload?.Id);
    }

    [Fact]
    public async Task SetSeries_returns_204()
    {
        // Arrange
        Mediator.Send(Arg.Any<SetBookSeriesCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<WorkId>(WorkId.New()));
        var client = _factory.CreateClient();
        var body = new SetBookSeriesRequest { SeriesName = "Dune", NumberInSeries = 1 };

        // Act
        var response = await client.PatchAsJsonAsync("/books/9780132350884/series", body);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private static BookDetails CreateBookDetails(Guid? id = null) => new()
    {
        BookId = id ?? Guid.NewGuid(),
        Title = "The Pragmatic Programmer",
        Authors = ["Andrew Hunt", "David Thomas"]
    };

    private sealed record ProblemPayload
    {
        public int? Status { get; init; }
        public string? Title { get; init; }
    }
}