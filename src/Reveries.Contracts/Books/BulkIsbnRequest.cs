namespace Reveries.Contracts.Books;

public class BulkIsbnRequest
{
    public List<string> Isbns { get; set; } = new();
}