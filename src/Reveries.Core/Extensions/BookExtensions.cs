using Reveries.Core.Entities;
using Reveries.Core.Enums;

namespace Reveries.Core.Extensions;

public static class BookExtensions
{
    public static Book WithDataSource(this Book book, DataSource dataSource)
    {
        ArgumentNullException.ThrowIfNull(book);

        var newBook = new Book { DataSource = dataSource, Title = book.Title};
        
        var props = typeof(Book).GetProperties()
            .Where(p => p.CanWrite && p.Name != nameof(Book.DataSource));

        foreach (var prop in props)
        {
            var value = prop.GetValue(book);
            prop.SetValue(newBook, value);
        }

        return newBook;
    }
}
