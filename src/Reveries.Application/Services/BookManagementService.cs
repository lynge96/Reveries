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

    public async Task<Book> SaveCompleteBookAsync(Book book, CancellationToken cancellationToken = default)
    {
        // 1. Handle Authors
        var authorNameVariantTasks = book.Authors.Select(author => 
            EnrichAuthorWithNameVariantsAsync(author, cancellationToken));
        
        await Task.WhenAll(authorNameVariantTasks);
        
        
        throw new NotImplementedException();
    }
    
    private async Task<Author> EnrichAuthorWithNameVariantsAsync(Author author, CancellationToken cancellationToken)
    {
        var variants = await _authorService.GetAuthorsByNameAsync(author.NormalizedName, cancellationToken);
    
        author.NameVariants = new List<AuthorNameVariant>
        {
            new AuthorNameVariant
            {
                NameVariant = author.NormalizedName,
                IsPrimary = true
            }
        };
    
        foreach(var variant in variants.Where(v => v != author.NormalizedName))
        {
            author.NameVariants.Add(new AuthorNameVariant 
            { 
                NameVariant = variant,
                IsPrimary = false
            });
        }
    
        return author;
    }

}