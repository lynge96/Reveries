using Reveries.Domain.Authors;
using Reveries.Domain.Publishers;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Works;
using Reveries.Persistence.Records;

namespace Reveries.Persistence.Mappers;

public static class RecordMappingExtensions
{
    public static PublisherRecord ToRecord(this Publisher publisher)
    {
        return new PublisherRecord
        {
            Id = publisher.Id.Value,
            Name = publisher.Name
        };
    }

    public static SeriesRecord ToRecord(this Series series)
    {
        return new SeriesRecord
        {
            Id = series.Id.Value,
            Name = series.Name
        };
    }

    public static AuthorRecord ToRecord(this Author author)
    {
        return new AuthorRecord
        {
            Id = author.Id.Value,
            Name = author.Name
        };
    }

    public static GenreRecord ToRecord(this Genre genre)
    {
        return new GenreRecord
        {
            Name = genre.Name
        };
    }

    public static DeweyDecimalRecord ToRecord(this DeweyDecimal deweyDecimal)
    {
        return new DeweyDecimalRecord
        {
            Code = deweyDecimal.Code
        };
    }
}