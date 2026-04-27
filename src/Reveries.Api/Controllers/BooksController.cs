using Mediator;
using Microsoft.AspNetCore.Mvc;
using Reveries.Api.Mappers;
using Reveries.Application.Books.Commands.SetBookSeries;
using Reveries.Application.Books.Models;
using Reveries.Application.Books.Queries.GetAllBooks;
using Reveries.Application.Books.Queries.GetBookById;
using Reveries.Application.Books.Queries.GetBookByIsbn;
using Reveries.Application.Books.Queries.GetBookExists;
using Reveries.Application.Books.Queries.GetBooksByIsbns;
using Reveries.Contracts.Books;
using Reveries.Core.ValueObjects;

namespace Reveries.Api.Controllers;

[ApiController]
[Route("books")]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDetailsDto>>> GetAllBooks([FromQuery] bool? isRead, CancellationToken ct)
    {
        var query = new GetAllBooksQuery { IsRead = isRead };
        var books = await _mediator.Send(query, ct);
        var booksDto = books.Select(b => b.ToDto());
        return Ok(booksDto);
    }
    
    [HttpGet("isbn/{isbn}")]
    public async Task<ActionResult<BookDetailsDto>> GetBookByIsbn(string isbn, CancellationToken ct)
    {
        var query = new GetBookByIsbnQuery { Isbn = Isbn.Create(isbn) };
        var book = await _mediator.Send(query, ct);
        var bookDto = book.ToDto();
        return Ok(bookDto);
    }

    [HttpGet("{isbn}/exists")]
    public async Task<ActionResult<bool>> BookExists(string isbn, CancellationToken ct)
    {
        var query = new GetBookExistsQuery { Isbn = Isbn.Create(isbn) };
        var exists = await _mediator.Send(query, ct);
        return Ok(exists);
    }

    [HttpPost("isbns")]
    public async Task<ActionResult<IEnumerable<BookDetailsDto>>> GetBooksByIsbns([FromBody] BulkIsbnRequest request, CancellationToken ct)
    {
        var query = new GetBooksByIsbnsQuery { Isbns = request.Isbns.Select(Isbn.Create).ToList() };
        var books = await _mediator.Send(query, ct);
        var booksDto = books.Select(b => b.ToDto());
        return Ok(booksDto);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookDetailsReadModel>> GetById(Guid id, CancellationToken ct)
    {
        var query = new GetBookByIdQuery { BookId = id };
        var book = await _mediator.Send(query, ct);
        var bookDto = book.ToDto();
        return Ok(bookDto);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest request, CancellationToken ct)
    {
        var command = request.ToCommand();
        var bookId = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = bookId.Value }, bookId.Value);
    }
    
    [HttpPatch("{isbn}/series")]
    public async Task<IActionResult> SetSeries([FromRoute] string isbn, [FromBody] SetBookSeriesRequest body, CancellationToken ct)
    {
        var command = new SetBookSeriesCommand
        {
            Isbn = Isbn.Create(isbn),
            SeriesName = body.SeriesName,
            NumberInSeries = body.NumberInSeries
        };
        await _mediator.Send(command, ct);
        return NoContent();
    }
}