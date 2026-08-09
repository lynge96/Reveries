using Reveries.Domain.Models;
using Reveries.Domain.ValueObjects;

namespace Reveries.Application.Books.Interfaces;

public interface IBookMergerService
{
    List<Book> AggregateBooksByIsbnsAsync(IReadOnlyList<Isbn> isbns, IReadOnlyList<Book>? isbndbBooks, IReadOnlyList<Book>? googleBooks);
    List<Book> AggregateBooksByTitlesAsync(IReadOnlyList<Title> titles, IReadOnlyList<Book>? isbndbBooks, IReadOnlyList<Book>? googleBooks);
}