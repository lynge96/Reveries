using Microsoft.Extensions.Logging;
using Reveries.Application.Interfaces.Cache;
using Reveries.Application.Interfaces.Messaging;
using Reveries.Core.Interfaces;
using Reveries.Core.Models;

namespace Reveries.Application.Queries.GetBookByIsbn;

public sealed class GetBookByIsbnHandler : IQueryHandler<GetBookByIsbnQuery, Book>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookCacheService _cache;
    private readonly ILogger<GetBookByIsbnHandler> _logger;
    
    public GetBookByIsbnHandler(
        IUnitOfWork unitOfWork, 
        IBookCacheService cache, 
        ILogger<GetBookByIsbnHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
        _logger = logger;
    }
    
    public Task<Book> Handle(GetBookByIsbnQuery query, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}