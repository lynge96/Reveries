using Microsoft.AspNetCore.Mvc;
using Reveries.Api.Mappers;
using Reveries.Application.Commands.CreateBook;
using Reveries.Application.Commands.SetBookSeries;
using Reveries.Application.Interfaces.Messaging;
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
    private readonly ICommandHandler<CreateBookCommand, int> _createBookHandler;
    private readonly ICommandHandler<SetBookSeriesCommand, int> _setSeriesHandler;
    private readonly IQueryHandler<GetBookByIsbnQuery, BookDetailsReadModel> _bookByIsbnHandler;
    private readonly IQueryHandler<GetBooksByIsbnsQuery, List<BookDetailsReadModel>> _booksByIsbnsHandler;
    private readonly IQueryHandler<GetBookByDbIdQuery, BookDetailsReadModel> _bookByIdHandler;
    private readonly IQueryHandler<GetAllBooksQuery, List<BookDetailsReadModel>> _getAllBooksHandler;

    public BooksController(
        ICommandHandler<CreateBookCommand, int> createBookHandler,
        ICommandHandler<SetBookSeriesCommand, int> setSeriesHandler,
        IQueryHandler<GetBookByIsbnQuery, BookDetailsReadModel> bookByIsbnHandler,
        IQueryHandler<GetBooksByIsbnsQuery, List<BookDetailsReadModel>> booksByIsbnsHandler,
        IQueryHandler<GetBookByDbIdQuery, BookDetailsReadModel> bookByIdHandler,
        IQueryHandler<GetAllBooksQuery, List<BookDetailsReadModel>> getAllBooksHandler)
    {
        _createBookHandler = createBookHandler;
        _setSeriesHandler = setSeriesHandler;
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
        
        var books = await _getAllBooksHandler.Handle(query, ct);
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

        var book = await _bookByIsbnHandler.Handle(query, ct);
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

        var books = await _booksByIsbnsHandler.Handle(query, ct);
        var booksDto = books.Select(b => b.ToDto()).ToList();

        return Ok(booksDto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDetailsReadModel>> GetById(int id, CancellationToken ct)
    {
        var query = new GetBookByDbIdQuery { DbId = id };
        
        var book = await _bookByIdHandler.Handle(query, ct);
        var bookDto = book.ToDto();
        
        return Ok(bookDto);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest request, CancellationToken ct)
    {
        var command = request.ToCommand();
        
        var bookId = await _createBookHandler.Handle(command, ct);

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

        await _setSeriesHandler.Handle(command, ct);
        return NoContent();
    }
}