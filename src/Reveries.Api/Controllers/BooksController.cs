using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Reveries.Api.Interfaces;
using Reveries.Contracts.DTOs;
using Reveries.Core.Validation;
using ValidationException = Reveries.Core.Exceptions.ValidationException;

namespace Reveries.Api.Controllers;

[ApiController]
[Route("api/v1/books")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly IValidator<CreateBookDto> _createBookValidator;

    public BooksController(IBookService bookService, IValidator<CreateBookDto> createBookValidator)
    {
        _bookService = bookService;
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
        IsbnValidator.NormalizeAndValidate(isbn, out var validatedIsbn);
        
        var book = await _bookService.GetBookByIsbnAsync(validatedIsbn, ct);
        
        return Ok(book);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDto>> GetById(int id, CancellationToken ct)
    {
        var book = await _bookService.GetBookByIdAsync(id, ct);
        
        return Ok(book);
    }
    
    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateBookDto bookData, CancellationToken ct)
    {
        var validationResult = await _createBookValidator.ValidateAsync(bookData, ct);
        
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var bookId = await _bookService.CreateBookAsync(bookData, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = bookId },
            bookId);
    }
}