using Microsoft.AspNetCore.Mvc;
using Reveries.Application.Interfaces.Services;
using Reveries.Contracts.Books;

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
    public async Task<ActionResult<BookDto>> GetBookByIsbn(string isbn)
    {
        var book = await _bookService.GetBookByIsbnAsync(isbn);
        
        return Ok(book);
    }
}