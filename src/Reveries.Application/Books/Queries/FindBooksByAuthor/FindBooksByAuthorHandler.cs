using Mediator;
using Microsoft.Extensions.Logging;
using Reveries.Application.Authors.Interfaces;
using Reveries.Application.Common.Abstractions;
using Reveries.Application.Common.Exceptions;
using Reveries.Core.Models;

namespace Reveries.Application.Books.Queries.FindBooksByAuthor;

public sealed class FindBooksByAuthorHandler : IQueryHandler<FindBooksByAuthorQuery, List<Book>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorSearch _authorSearch;
    private readonly ILogger<FindBooksByAuthorHandler> _logger;
    
    public FindBooksByAuthorHandler(
        IUnitOfWork unitOfWork,
        IAuthorSearch authorSearch,
        ILogger<FindBooksByAuthorHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _authorSearch = authorSearch;
        _logger = logger;
    }
    
    public async ValueTask<List<Book>> Handle(FindBooksByAuthorQuery query, CancellationToken ct)
    {
        var author = query.Author;
        
        var databaseBooks = await _unitOfWork.Books.GetBooksByAuthorAsync(author, ct);
        if (databaseBooks.Count > 0)
            return databaseBooks;
        
        var apiBooks = await _authorSearch.GetBooksByAuthorAsync(author, ct);

        if (apiBooks is null)
            throw new NotFoundException($"Books with author '{author}' were not found.");
        
        _logger.LogInformation(
            "Book lookup by Author completed. Requested '{Author}'. DB: {DbCount}, API: {ApiCount}. Final: {Total}.",
            author.NormalizedName,
            databaseBooks.Count,
            apiBooks.Count,
            databaseBooks.Count + apiBooks.Count
        );
        
        return apiBooks;
    }
}