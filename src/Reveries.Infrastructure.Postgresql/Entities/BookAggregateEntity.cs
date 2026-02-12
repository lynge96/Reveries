namespace Reveries.Infrastructure.Postgresql.Entities;

public class BookAggregateEntity
{
    public BookEntity Book { get; set; } = null!;
    public PublisherEntity? Publisher { get; set; }
    public List<AuthorEntity>? Authors { get; set; } = new();
    public List<GenreEntity>? Genres { get; set; } = new();
    public List<DeweyDecimalEntity>? DeweyDecimals { get; set; } = new();
    public SeriesEntity? Series { get; set; }
}