using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Reveries.Api.Mappers;
using Reveries.Application.Commands.CreateBook;
using Reveries.Application.Commands.SetBookSeries;
using Reveries.Application.Interfaces.Messaging;
using Reveries.Application.Queries;
using Reveries.Application.Queries.GetBookByIsbn;
using Reveries.Contracts.Books;
using Reveries.Contracts.Requests;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using ValidationException = Reveries.Application.Exceptions.ValidationException;

namespace Reveries.Api.Controllers;

[ApiController]
[Route("api/v1/books")]
public class BooksController : ControllerBase
{
    private readonly ICommandHandler<CreateBookCommand, int> _createBookHandler;
    private readonly ICommandHandler<SetBookSeriesCommand, int> _setSeriesHandler;
    private readonly IQueryHandler<GetBookByIsbnQuery, Book> _bookByIsbnHandler;
    private readonly IValidator<CreateBookRequest> _createBookValidator;

    public BooksController(
        ICommandHandler<CreateBookCommand, int> createBookHandler,
        ICommandHandler<SetBookSeriesCommand, int> setSeriesHandler,
        IQueryHandler<GetBookByIsbnQuery, Book> bookByIsbnHandler,
        IValidator<CreateBookRequest> createBookValidator)
    {
        _createBookHandler = createBookHandler;
        _setSeriesHandler = setSeriesHandler;
        _bookByIsbnHandler = bookByIsbnHandler;
        _createBookValidator = createBookValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDetailsReadModel>>> GetBooks([FromQuery] bool? isRead)
    {
        var books = await _bookService.GetAllBooksAsync();
        
        if (isRead.HasValue)
            books = books.Where(b => b.IsRead == isRead.Value);

        return Ok(books);
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
    public async Task<ActionResult<List<BookDetailsReadModel>>> GetByIsbns([FromBody] BulkIsbnRequest request, CancellationToken ct)
    {
        var isbns = request.Isbns.Select(Isbn.Create).ToList();
        
        if (isbns.Count == 0)
            return BadRequest("At least one ISBN must be provided.");
        
        var books = await _bookService.GetBooksByIsbnsAsync(isbns, ct);
            
        return Ok(books);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDetailsReadModel>> GetById(int id, CancellationToken ct)
    {
        var book = await _bookService.GetBookByIdAsync(id, ct);
        
        return Ok(book);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest request, CancellationToken ct)
    {
        var validationResult = await _createBookValidator.ValidateAsync(request, ct);
        
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
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