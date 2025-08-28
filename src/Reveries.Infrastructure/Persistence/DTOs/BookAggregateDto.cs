namespace Reveries.Infrastructure.Persistence.DTOs;

public class BookAggregateDto
{
    public BookDto Book { get; set; } = null!;
    public PublisherDto? Publisher { get; set; }
    public List<AuthorDto> Authors { get; set; } = new();
    public List<SubjectDto> Subjects { get; set; } = new();
    public DimensionsDto? Dimensions { get; set; }
    public List<DeweyDecimalDto> DeweyDecimals { get; set; } = new();
    public SeriesDto? Series { get; set; } 
}