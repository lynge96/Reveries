using Microsoft.AspNetCore.Mvc;
using Reveries.Api.Mappers;
using Reveries.Application.Books.Commands.CreateBook;
using Reveries.Application.Books.Commands.SetBookSeries;
using Reveries.Application.Books.Models;
using Reveries.Application.Books.Queries.GetAllBooks;
using Reveries.Application.Books.Queries.GetBookByDbId;
using Reveries.Application.Books.Queries.GetBookByIsbn;
using Reveries.Application.Books.Queries.GetBookExists;
using Reveries.Application.Books.Queries.GetBooksByIsbns;
using Reveries.Contracts.Books;
using Reveries.Core.ValueObjects;

namespace Reveries.Api.Controllers;

[ApiController]
[Route("api/v1/books")]
public class BooksController : ControllerBase
{
    private readonly CreateBookHandler _createBookHandler;
    private readonly SetBookSeriesHandler _setBookSeriesHandler;
    private readonly GetBookByIsbnHandler _getBookByIsbnHandler;
    private readonly GetBooksByIsbnsHandler _getBooksByIsbnsHandler;
    private readonly GetBookByDbIdHandler _getBookByIdHandler;
    private readonly GetAllBooksHandler _getAllBooksHandler;
    private readonly GetBookExistsHandler _getBookExistsHandler;

    public BooksController(
        CreateBookHandler createBookHandler,
        SetBookSeriesHandler setBookSeriesHandler,
        GetBookByIsbnHandler getBookByIsbnHandler,
        GetBooksByIsbnsHandler getBooksByIsbnsHandler,
        GetBookByDbIdHandler getBookByIdHandler,
        GetAllBooksHandler getAllBooksHandler,
        GetBookExistsHandler getBookExistsHandler)
    {
        _createBookHandler = createBookHandler;
        _setBookSeriesHandler = setBookSeriesHandler;
        _getBookByIsbnHandler = getBookByIsbnHandler;
        _getBooksByIsbnsHandler = getBooksByIsbnsHandler;
        _getBookByIdHandler = getBookByIdHandler;
        _getAllBooksHandler = getAllBooksHandler;
        _getBookExistsHandler = getBookExistsHandler;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDetailsDto>>> GetAllBooks([FromQuery] bool? isRead, CancellationToken ct)
    {
        var query = new GetAllBooksQuery { IsRead = isRead };
        var books = await _getAllBooksHandler.HandleAsync(query, ct);
        var booksDto = books.Select(b => b.ToDto()).ToList();
        return Ok(booksDto);
    }
    
    [HttpGet("{isbn}")]
    public async Task<ActionResult<BookDetailsDto>> GetBookByIsbn(string isbn, CancellationToken ct)
    {
        var query = new GetBookByIsbnQuery { Isbn = Isbn.Create(isbn) };
        var book = await _getBookByIsbnHandler.HandleAsync(query, ct);
        var bookDto = book.ToDto();
        return Ok(bookDto);
    }

    [HttpGet("{isbn}/exists")]
    public async Task<ActionResult<bool>> BookExists(string isbn, CancellationToken ct)
    {
        var query = new GetBookExistsQuery { Isbn = Isbn.Create(isbn) };
        var exists = await _getBookExistsHandler.HandleAsync(query, ct);
        return Ok(exists);
    }

    [HttpPost("isbns")]
    public async Task<ActionResult<List<BookDetailsDto>>> GetBooksByIsbns([FromBody] BulkIsbnRequest request, CancellationToken ct)
    {
        var query = new GetBooksByIsbnsQuery { Isbns = request.Isbns.Select(Isbn.Create).ToList() };
        var books = await _getBooksByIsbnsHandler.HandleAsync(query, ct);
        var booksDto = books.Select(b => b.ToDto()).ToList();
        return Ok(booksDto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDetailsReadModel>> GetById(int id, CancellationToken ct)
    {
        var query = new GetBookByDbIdQuery { DbId = id };
        var book = await _getBookByIdHandler.HandleAsync(query, ct);
        var bookDto = book.ToDto();
        return Ok(bookDto);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest request, CancellationToken ct)
    {
        var command = request.ToCommand();
        var bookId = await _createBookHandler.HandleAsync(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = bookId }, bookId);
    }
    
    [HttpPatch("books/{isbn}/series")]
    public async Task<IActionResult> SetSeries([FromRoute] string isbn, [FromBody] SetBookSeriesRequest body, CancellationToken ct)
    {
        var command = new SetBookSeriesCommand
        {
            Isbn = Isbn.Create(isbn),
            SeriesName = body.SeriesName,
            NumberInSeries = body.NumberInSeries
        };
        await _setBookSeriesHandler.HandleAsync(command, ct);
        return NoContent();
    }
}