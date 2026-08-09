using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Interfaces;

public interface IBookMergerService
{
    List<Book> AggregateBooksByIsbnsAsync(IReadOnlyList<Isbn> isbns, IReadOnlyList<Book>? isbndbBooks, IReadOnlyList<Book>? googleBooks);
    List<Book> AggregateBooksByTitlesAsync(IReadOnlyList<Title> titles, IReadOnlyList<Book>? isbndbBooks, IReadOnlyList<Book>? googleBooks);
}