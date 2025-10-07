using Microsoft.AspNetCore.Mvc;
using Reveries.Application.Interfaces.Services;
using Reveries.Contracts.Books;

namespace Reveries.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }
    
    [HttpGet("{isbn}")]
    public async Task<ActionResult<BookDto>> GetBookByIsbn(string isbn)
    {
        var book = await _bookService.GetBookByIsbnAsync(isbn);
        return Ok(book);
    }
}