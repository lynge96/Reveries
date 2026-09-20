using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Reveries.Api.Mappers;
using Reveries.Application.Books.Queries.FindBookByIsbn;
using Reveries.Application.Books.Queries.FindBooksByIsbns;
using Reveries.Application.Books.Queries.GetAllBooks;
using Reveries.Application.Books.Queries.GetBookById;
using Reveries.Application.Books.Queries.GetBookExists;
using Reveries.Api.Contracts.Books.Dtos;
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

        group.MapGet("/{id:guid}", GetBookById)
            .WithName("GetBookById")
            .WithSummary("Get book by ID")
            .WithDescription("Fetches a book from the database by ID")
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/isbn/{isbn}", GetBookByIsbn)
            .WithName("GetBookByIsbn")
            .WithSummary("Get book by ISBN")
            .WithDescription("Fetches a specific book by ISBN from external APIs, cache or the database")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status502BadGateway);

        group.MapGet("/isbn/{isbn}/exists", BookExists)
            .WithName("BookExists")
            .WithSummary("Check book exists")
            .WithDescription("Checks if a book with the specified ISBN exists in the database")
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/isbns", GetBooksByIsbns)
            .WithName("GetBooksByIsbns")
            .WithSummary("Get books by ISBNs")
            .WithDescription("Fetches multiple books by ISBNs")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status502BadGateway);

        group.MapPost("/", CreateBook)
            .WithName("CreateBook")
            .WithSummary("Add book")
            .WithDescription("Adds a new book to the database")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        return app;
    }

    private static async Task<Ok<BooksResponse>> GetAllBooks(IMediator mediator, CancellationToken ct)
    {
        var books = await mediator.Send(new GetAllBooksQuery(), ct);
        return TypedResults.Ok(books.ToResponse());
    }

    private static async Task<Ok<BookDetailsDto>> GetBookById(Guid id, IMediator mediator, CancellationToken ct)
    {
        var book = await mediator.Send(new GetBookByIdQuery(id), ct);
        return TypedResults.Ok(book.ToDto());
    }

    private static async Task<Ok<BookDetailsDto>> GetBookByIsbn(string isbn, IMediator mediator, CancellationToken ct)
    {
        var book = await mediator.Send(new FindBookByIsbnQuery(isbn), ct);
        return TypedResults.Ok(book.ToDto());
    }

    private static async Task<Ok<bool>> BookExists(string isbn, IMediator mediator, CancellationToken ct)
    {
        var exists = await mediator.Send(new GetBookExistsQuery(isbn), ct);
        return TypedResults.Ok(exists);
    }

    private static async Task<Ok<BooksResponse>> GetBooksByIsbns(BulkIsbnRequest request, IMediator mediator, CancellationToken ct)
    {
        var books = await mediator.Send(new FindBooksByIsbnsQuery(request.Isbns), ct);
        return TypedResults.Ok(books.ToResponse());
    }

    private static async Task<CreatedAtRoute<CreateBookResponse>> CreateBook(CreateBookRequest request, IMediator mediator, CancellationToken ct)
    {
        var editionId = await mediator.Send(request.ToCommand(), ct);
        return TypedResults.CreatedAtRoute(
            new CreateBookResponse(editionId.Value),
            "GetBookById",
            new { id = editionId.Value });
    }
}