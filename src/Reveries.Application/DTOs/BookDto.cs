namespace Reveries.Application.DTOs;

public class BookDto
{
    public string Title { get; set; }
    
    public string Image { get; set; }
    
    public string ImageOriginal { get; set; }
    
    public string TitleLong { get; set; }
    
    public string DatePublished { get; set; }
    
    public string Publisher { get; set; }
    
    public string Synopsis { get; set; }
    
    public List<string> Subjects { get; set; }
    
    public List<string> Authors { get; set; }
    
    public string Isbn13 { get; set; }
    
    public string Isbn { get; set; }
    
    public string Isbn10 { get; set; }
    
    public string Edition { get; set; }
    
    public string Binding { get; set; }
    
    public int? Msrp { get; set; }
    
    public string Language { get; set; }
    
    public string Dimensions { get; set; }
    
    public int? Pages { get; set; }
    
    public DimensionsStructuredDto DimensionsStructured { get; set; }
}