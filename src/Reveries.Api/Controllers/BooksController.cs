using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Reveries.Api.Mappers;
using Reveries.Application.Commands;
using Reveries.Application.Commands.CreateBook;
using Reveries.Application.Interfaces.Messaging;
using Reveries.Contracts.Books;
using Reveries.Contracts.DTOs;
using Reveries.Contracts.Requests;
using Reveries.Core.ValueObjects;
using ValidationException = Reveries.Application.Exceptions.ValidationException;

namespace Reveries.Api.Controllers;

[ApiController]
[Route("api/v1/books")]
public class BooksController : ControllerBase
{
    private readonly ICommandHandler<CreateBookCommand> _createBookHandler;
    private readonly IValidator<CreateBookRequest> _createBookValidator;

    public BooksController(ICommandHandler<CreateBookCommand> createBookHandler, IValidator<CreateBookRequest> createBookValidator)
    {
        _createBookHandler = createBookHandler;
        _createBookValidator = createBookValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks([FromQuery] bool? isRead)
    {
        var books = await _bookService.GetAllBooksAsync();
        
        if (isRead.HasValue)
            books = books.Where(b => b.IsRead == isRead.Value);

        return Ok(books);
    }
    
    [HttpGet("{isbn}")]
    public async Task<ActionResult<BookDto>> GetByIsbn(string isbn, CancellationToken ct)
    {
        var validIsbn = Isbn.Create(isbn);
        
        var book = await _bookService.GetBookByIsbnAsync(validIsbn, ct);
        
        return Ok(book);
    }

    [HttpPost("isbns")]
    public async Task<ActionResult<List<BookDto>>> GetByIsbns([FromBody] BulkIsbnRequest request, CancellationToken ct)
    {
        var isbns = request.Isbns.Select(Isbn.Create).ToList();
        
        if (isbns.Count == 0)
            return BadRequest("At least one ISBN must be provided.");
        
        var books = await _bookService.GetBooksByIsbnsAsync(isbns, ct);
            
        return Ok(books);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDto>> GetById(int id, CancellationToken ct)
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
}