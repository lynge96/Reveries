using Reveries.Core.Models;
using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Mappers;

public static class BookAggregateMapperExtensions
{
    public static Book MapAggregateDtoToDomain(this BookAggregateEntity entity)
    {
        var book = entity.Book.ToDomain();

        book.Publisher = entity.Publisher?.ToDomain();
        book.Series = entity.Series?.ToDomain();
        book.Dimensions = entity.Dimensions?.ToDomain();

        book.Authors = entity.Authors.Select(a => a.ToDomain()).ToList();
        book.Subjects = entity.Subjects.Select(s => s.ToDomain()).ToList();
        book.DeweyDecimals = entity.DeweyDecimals.Select(dd => dd.ToDomain()).ToList();

        return book;
    }
}