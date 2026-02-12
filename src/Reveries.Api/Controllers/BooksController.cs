using Microsoft.AspNetCore.Mvc;
using Reveries.Api.Mappers;
using Reveries.Application.Commands.CreateBook;
using Reveries.Application.Commands.SetBookSeries;
using Reveries.Application.Queries;
using Reveries.Application.Queries.GetAllBooks;
using Reveries.Application.Queries.GetBookByDbId;
using Reveries.Application.Queries.GetBookByIsbn;
using Reveries.Application.Queries.GetBookByIsbns;
using Reveries.Contracts.Books;
using Reveries.Core.ValueObjects;

namespace Reveries.Api.Controllers;

[ApiController]
[Route("api/v1/books")]
public class BooksController : ControllerBase
{
    private readonly CreateBookHandler _createBookHandler;
    private readonly SetBookSeriesHandler _setBookSeriesHandler;
    private readonly GetBookByIsbnHandler _bookByIsbnHandler;
    private readonly GetBooksByIsbnsHandler _booksByIsbnsHandler;
    private readonly GetBookByDbIdHandler _bookByIdHandler;
    private readonly GetAllBooksHandler _getAllBooksHandler;

    public BooksController(
        CreateBookHandler createBookHandler,
        SetBookSeriesHandler setBookSeriesHandler,
        GetBookByIsbnHandler bookByIsbnHandler,
        GetBooksByIsbnsHandler booksByIsbnsHandler,
        GetBookByDbIdHandler bookByIdHandler,
        GetAllBooksHandler getAllBooksHandler)
    {
        _createBookHandler = createBookHandler;
        _setBookSeriesHandler = setBookSeriesHandler;
        _bookByIsbnHandler = bookByIsbnHandler;
        _booksByIsbnsHandler = booksByIsbnsHandler;
        _bookByIdHandler = bookByIdHandler;
        _getAllBooksHandler = getAllBooksHandler;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDetailsDto>>> GetAllBooks([FromQuery] bool? isRead, CancellationToken ct)
    {
        var query = new GetAllBooksQuery
        {
            IsRead = isRead
        };
        
        var books = await _getAllBooksHandler.HandleAsync(query, ct);
        var booksDto = books.Select(b => b.ToDto()).ToList();

        return Ok(booksDto);
    }
    
    [HttpGet("{isbn}")]
    public async Task<ActionResult<BookDetailsDto>> GetBookByIsbn(string isbn, CancellationToken ct)
    {
        var query = new GetBookByIsbnQuery
        {
            Isbn = Isbn.Create(isbn)
        };

        var book = await _bookByIsbnHandler.HandleAsync(query, ct);
        var bookDto = book.ToDto();

        return Ok(bookDto);
    }

    [HttpPost("isbns")]
    public async Task<ActionResult<List<BookDetailsDto>>> GetBooksByIsbns([FromBody] BulkIsbnRequest request, CancellationToken ct)
    {
        var query = new GetBooksByIsbnsQuery
        {
            Isbns = request.Isbns.Select(Isbn.Create).ToList()
        };

        var books = await _booksByIsbnsHandler.HandleAsync(query, ct);
        var booksDto = books.Select(b => b.ToDto()).ToList();

        return Ok(booksDto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDetailsReadModel>> GetById(int id, CancellationToken ct)
    {
        var query = new GetBookByDbIdQuery { DbId = id };
        
        var book = await _bookByIdHandler.HandleAsync(query, ct);
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