using Mediator;
using Microsoft.AspNetCore.Mvc;
using Reveries.Api.Mappers;
using Reveries.Application.Books.Commands.SetBookSeries;
using Reveries.Application.Books.Queries.FindBookByIsbn;
using Reveries.Application.Books.Queries.FindBooksByIsbns;
using Reveries.Application.Books.Queries.GetAllBooks;
using Reveries.Application.Books.Queries.GetBookById;
using Reveries.Application.Books.Queries.GetBookExists;
using Reveries.Contracts.Books.Dtos;
using Reveries.Contracts.Books.Requests;
using Reveries.Contracts.Books.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace Reveries.Api.Controllers;

[ApiController]
[Route("books")]
[Produces("application/json")]
[Consumes("application/json")]
[Tags("Books")]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all books",
        Description = "Fetches every book in the database",
        OperationId = "Books_GetAll")]
    [ProducesResponseType(typeof(BooksResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BooksResponse>> GetAllBooks(
        [FromQuery] GetAllBooksRequest request, 
        CancellationToken ct)
    {
        var query = new GetAllBooksQuery(request.IsRead);
        var books = await _mediator.Send(query, ct);

        return Ok(books.ToResponse());
    }
    
    [HttpGet("isbn/{isbn}")]
    [SwaggerOperation(
        Summary = "Get book by ISBN",
        Description = "Fetches a specific book by ISBN from external APIs, cache or the database",
        OperationId = "Books_GetByIsbn")]
    [ProducesResponseType(typeof(BookDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDetailsDto>> GetBookByIsbn(string isbn, CancellationToken ct)
    {
        var query = new FindBookByIsbnQuery(isbn);
        var book = await _mediator.Send(query, ct);

        return Ok(book.ToDto());
    }

    [HttpGet("isbn/{isbn}/exists")]
    [SwaggerOperation(
        Summary = "Check book exists",
        Description = "Checks if a book with the specified ISBN exists in the database",
        OperationId = "Books_CheckExists")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> BookExists(string isbn, CancellationToken ct)
    {
        var query = new GetBookExistsQuery(isbn);
        var exists = await _mediator.Send(query, ct);
        
        return Ok(exists);
    }

    [HttpPost("isbns")]
    [SwaggerOperation(
        Summary = "Get books by ISBNs",
        Description = "Fetches multiple books by ISBNs",
        OperationId = "Books_GetByIsbns")]
    [ProducesResponseType(typeof(BooksResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BooksResponse>> GetBooksByIsbns([FromBody] BulkIsbnRequest request, CancellationToken ct)
    {
        var query = new FindBooksByIsbnsQuery(request.Isbns);
        var books = await _mediator.Send(query, ct);

        return Ok(books.ToResponse());
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Get book by ID",
        Description = "Fetches a book from the database by ID",
        OperationId = "Books_ById")]
    [ProducesResponseType(typeof(BookDetailsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BookDetailsDto>> GetById(Guid id, CancellationToken ct)
    {
        var query = new GetBookByIdQuery(id);
        var book = await _mediator.Send(query, ct);

        return Ok(book.ToDto());
    }
    
    [HttpPost]
    [SwaggerOperation(
        Summary = "Add book",
        Description = "Adds a new book to the database",
        OperationId = "Books_Create")]
    [ProducesResponseType(typeof(CreateBookResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CreateBookResponse>> Create([FromBody] CreateBookRequest request, CancellationToken ct)
    {
        var command = request.ToCommand();
        var bookId = await _mediator.Send(command, ct);
        
        return new CreateBookResponse(bookId.Value);
    }
    
    [HttpPatch("{isbn}/series")]
    [SwaggerOperation(
        Summary = "Set series",
        Description = "Sets the series for a book by its ISBN",
        OperationId = "Books_SetSeries")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetSeries([FromRoute] string isbn, [FromBody] SetBookSeriesRequest body, CancellationToken ct)
    {
        var command = new SetBookSeriesCommand(isbn, body.SeriesName, body.NumberInSeries);

        await _mediator.Send(command, ct);
        return NoContent();
    }
}