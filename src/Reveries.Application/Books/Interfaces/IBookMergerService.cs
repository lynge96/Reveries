using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Interfaces;

public interface IBookMergerService
{
    List<BookCandidate> AggregateBooksByIsbnsAsync(IReadOnlyList<Isbn> isbns, IReadOnlyList<BookCandidate>? isbndbBooks, IReadOnlyList<BookCandidate>? googleBooks);
    List<BookCandidate> AggregateBooksByTitlesAsync(IReadOnlyList<Title> titles, IReadOnlyList<BookCandidate>? isbndbBooks, IReadOnlyList<BookCandidate>? googleBooks);
}