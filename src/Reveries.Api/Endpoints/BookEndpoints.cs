using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Reveries.Api.Mappers;
using Reveries.Application.BookSeries.Commands.SetBookSeries;
using Reveries.Application.Books.Queries.FindBookByIsbn;
using Reveries.Application.Books.Queries.FindBooksByIsbns;
using Reveries.Application.Books.Queries.GetAllBooks;
using Reveries.Application.Books.Queries.GetBookById;
using Reveries.Application.Books.Queries.GetBookExists;
using Reveries.Contracts.Books.Dtos;
using Reveries.Contracts.Books.Requests;
using Reveries.Contracts.Books.Responses;

namespace Reveries.Api.Endpoints;

public static class BookEndpoints
{
    public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("books")
            .WithTags("Books");

        group.MapGet("/", GetAllBooks)
            .WithSummary("Get all books")
            .WithDescription("Fetches every book in the database");

        group.MapGet("/{id:guid}", GetBookById)
            .WithSummary("Get book by ID")
            .WithDescription("Fetches a book from the database by ID")
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/isbn/{isbn}", GetBookByIsbn)
            .WithSummary("Get book by ISBN")
            .WithDescription("Fetches a specific book by ISBN from external APIs, cache or the database")
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/isbn/{isbn}/exists", BookExists)
            .WithSummary("Check book exists")
            .WithDescription("Checks if a book with the specified ISBN exists in the database");

        group.MapPost("/isbns", GetBooksByIsbns)
            .WithSummary("Get books by ISBNs")
            .WithDescription("Fetches multiple books by ISBNs");

        group.MapPost("/", CreateBook)
            .WithSummary("Add book")
            .WithDescription("Adds a new book to the database");

        group.MapPatch("/{isbn}/series", SetSeries)
            .WithSummary("Set series")
            .WithDescription("Sets the series for a book by its ISBN");

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

    private static async Task<Created<CreateBookResponse>> CreateBook(CreateBookRequest request, IMediator mediator, CancellationToken ct)
    {
        var editionId = await mediator.Send(request.ToCommand(), ct);
        return TypedResults.Created($"/books/{editionId.Value}", new CreateBookResponse(editionId.Value));
    }

    private static async Task<NoContent> SetSeries(string isbn, SetBookSeriesRequest body, IMediator mediator, CancellationToken ct)
    {
        await mediator.Send(new SetBookSeriesCommand(isbn, body.SeriesName, body.NumberInSeries), ct);
        return TypedResults.NoContent();
    }
}