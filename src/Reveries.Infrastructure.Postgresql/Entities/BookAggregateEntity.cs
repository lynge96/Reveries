namespace Reveries.Infrastructure.Postgresql.Entities;


public class BookAggregateEntity
{
    public BookEntity Book { get; set; } = null!;
    public PublisherEntity? Publisher { get; set; }
    public List<AuthorEntity> Authors { get; set; } = new();
    public List<SubjectEntity> Subjects { get; set; } = new();
    public DimensionsEntity? Dimensions { get; set; }
    public List<DeweyDecimalEntity> DeweyDecimals { get; set; } = new();
    public SeriesEntity? Series { get; set; }
}