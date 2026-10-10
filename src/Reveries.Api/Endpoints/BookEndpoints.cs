using System.ComponentModel;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Reveries.Api.Mappers;
using Reveries.Application.Books.Queries.FindBookByIsbn;
using Reveries.Application.Books.Queries.FindBooksByIsbns;
using Reveries.Application.Books.Queries.GetAllBooks;
using Reveries.Application.Books.Queries.GetBookExists;
using Reveries.Api.Configuration.RequestTimeouts;
using Reveries.Api.Contracts.Books.Requests;
using Reveries.Api.Contracts.Books.Responses;

namespace Reveries.Api.Endpoints;

public static class BookEndpoints
{
    public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("books")
            .WithTags("Books")
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/", GetAllBooks)
            .WithName("GetAllBooks")
            .WithSummary("Get all books")
            .WithDescription("Fetches every book in the database");

        group.MapGet("/isbn/{isbn}", GetBookByIsbn)
            .WithName("GetBookByIsbn")
            .WithSummary("Get book by ISBN")
            .WithDescription("Fetches a specific book by ISBN from external APIs, the cache or the database. Returns 404 when no book is found.")
            .WithRequestTimeout(RequestTimeoutExtensions.ExternalLookupPolicy)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status502BadGateway)
            .ProducesProblem(StatusCodes.Status504GatewayTimeout);

        group.MapGet("/isbn/{isbn}/exists", BookExists)
            .WithName("BookExists")
            .WithSummary("Check book exists")
            .WithDescription("Checks if a book with the specified ISBN exists in the database")
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/isbns", GetBooksByIsbns)
            .WithName("GetBooksByIsbns")
            .WithSummary("Get books by ISBNs")
            .WithDescription("Looks up multiple books by ISBN from external APIs, the cache or the database. Returns 404 when none of the ISBNs resolve to a book.")
            .WithRequestTimeout(RequestTimeoutExtensions.ExternalLookupPolicy)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status502BadGateway)
            .ProducesProblem(StatusCodes.Status504GatewayTimeout);

        group.MapPost("/", CreateBook)
            .WithName("CreateBook")
            .WithSummary("Add book")
            .WithDescription("Adds a new book to the database")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        return app;
    }

    private static async Task<Ok<BookCollectionResponse>> GetAllBooks(IMediator mediator, CancellationToken ct)
    {
        var books = await mediator.Send(new GetAllBooksQuery(), ct);
        return TypedResults.Ok(books.ToCollectionResponse());
    }

    private static async Task<Ok<BookResponse>> GetBookByIsbn(
        [Description("ISBN-10 or ISBN-13 of the edition.")] string isbn,
        IMediator mediator,
        CancellationToken ct)
    {
        var book = await mediator.Send(new FindBookByIsbnQuery(isbn), ct);
        return TypedResults.Ok(book.ToResponse());
    }

    private static async Task<Ok<BookExistsResponse>> BookExists(
        [Description("ISBN-10 or ISBN-13 of the edition.")] string isbn,
        IMediator mediator,
        CancellationToken ct)
    {
        var exists = await mediator.Send(new GetBookExistsQuery(isbn), ct);
        return TypedResults.Ok(new BookExistsResponse(exists));
    }

    private static async Task<Ok<BookCollectionResponse>> GetBooksByIsbns(
        BookLookupRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var books = await mediator.Send(new FindBooksByIsbnsQuery(request.Isbns), ct);
        return TypedResults.Ok(books.ToCollectionResponse());
    }

    private static async Task<Created<CreateBookResponse>> CreateBook(CreateBookRequest request, IMediator mediator, CancellationToken ct)
    {
        var editionId = await mediator.Send(request.ToCommand(), ct);
        var response = new CreateBookResponse(editionId.Value);

        var isbn = request.Isbn13 ?? request.Isbn10;
        var location = isbn is null ? null : $"/books/isbn/{Uri.EscapeDataString(isbn)}";

        return TypedResults.Created(location, response);
    }
}
