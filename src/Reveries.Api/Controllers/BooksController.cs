using Microsoft.AspNetCore.Mvc;
using Reveries.Api.Interfaces;
using Reveries.Contracts.DTOs;
using Reveries.Core.Validation;

namespace Reveries.Api.Controllers;

[ApiController]
[Route("api/v1/books")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
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
        IsbnValidator.TryValidate(isbn, out var validatedIsbn);
        
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
    public async Task<ActionResult<int>> Create([FromBody] BookDto bookData, CancellationToken ct)
    {
        var bookId = await _bookService.CreateBookAsync(bookData, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = bookId },
            bookId);
    }
}