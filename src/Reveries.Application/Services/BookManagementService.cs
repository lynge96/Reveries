using Reveries.Application.Interfaces.Services;
using Reveries.Core.Entities;

namespace Reveries.Application.Services;

public class BookManagementService : IBookManagementService
{
    private readonly IAuthorService _authorService;
    private readonly IPublisherService _publisherService;
    private readonly ISubjectService _subjectService;
    private readonly IBookService _bookService;

    public BookManagementService(IAuthorService authorService, IPublisherService publisherService, ISubjectService subjectService, IBookService bookService)
    {
        _authorService = authorService;
        _publisherService = publisherService;
        _subjectService = subjectService;
        _bookService = bookService;
    }

    public Task<Book> SaveCompleteBookAsync(Book book, CancellationToken cancellationToken = default)
    {
        // 1. Handle Authors
        
        // Sætte fornavn og efternavn og NormalizedName for forfatterne
        // Derefter skal alle navnevarianter findes i databasen og gemmes i listen.
        
        throw new NotImplementedException();
    }
}