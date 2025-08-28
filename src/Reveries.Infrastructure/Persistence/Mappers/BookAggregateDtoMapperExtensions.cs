using Reveries.Core.Entities;
using Reveries.Infrastructure.Persistence.DTOs;

namespace Reveries.Infrastructure.Persistence.Mappers;

public static class BookAggregateMapperExtensions
{
    public static Book MapAggregateDtoToDomain(this BookAggregateDto dto)
    {
        var book = dto.Book.ToDomain();

        book.Publisher = dto.Publisher?.ToDomain();
        book.Series = dto.Series?.ToDomain();
        book.Dimensions = dto.Dimensions?.ToDomain();

        book.Authors = dto.Authors.Select(a => a.ToDomain()).ToList();
        book.Subjects = dto.Subjects.Select(s => s.ToDomain()).ToList();
        book.DeweyDecimals = dto.DeweyDecimals.Select(dd => dd.ToDomain()).ToList();

        return book;
    }
}