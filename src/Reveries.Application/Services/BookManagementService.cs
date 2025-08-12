using Reveries.Application.Interfaces.Services;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces;

namespace Reveries.Application.Services;

public class BookManagementService : IBookManagementService
{
    private readonly IAuthorService _authorService;
    private readonly IUnitOfWork _unitOfWork;

    public BookManagementService(IAuthorService authorService, IUnitOfWork unitOfWork)
    {
        _authorService = authorService;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<int> SaveCompleteBookAsync(Book book, CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // 1. Håndter forfattere
            var authorNameVariantTasks = book.Authors.Select(EnrichAuthorWithNameVariantsAsync);
            await Task.WhenAll(authorNameVariantTasks);

            // 2. Gem bogen
            var savedBookId = await _unitOfWork.Books.CreateBookAsync(book);
            
            // TODO: Gem forfattere, subject og author
            
            // 3. Commit transaktionen
            await _unitOfWork.CommitAsync();

            return savedBookId;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
    
    private async Task<Author> EnrichAuthorWithNameVariantsAsync(Author author)
    {
        var variants = await _authorService.GetAuthorsByNameAsync(author.NormalizedName);
    
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