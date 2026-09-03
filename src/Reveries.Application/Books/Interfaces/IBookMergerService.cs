using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Interfaces;

public interface IBookMergerService
{
    List<EditionWithWork> AggregateBooksByIsbnsAsync(IReadOnlyList<Isbn> isbns, IReadOnlyList<EditionWithWork>? isbndbBooks, IReadOnlyList<EditionWithWork>? googleBooks);
    List<EditionWithWork> AggregateBooksByTitlesAsync(IReadOnlyList<Title> titles, IReadOnlyList<EditionWithWork>? isbndbBooks, IReadOnlyList<EditionWithWork>? googleBooks);
}