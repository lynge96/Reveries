using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Interfaces;

public interface IBookMergerService
{
    List<BookCandidate> AggregateBooksByIsbns(IReadOnlyList<Isbn> isbns, IReadOnlyList<SourcedBooks> sources);
}