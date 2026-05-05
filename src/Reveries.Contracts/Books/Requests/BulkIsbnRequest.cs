namespace Reveries.Contracts.Books.Requests;

public class BulkIsbnRequest
{
    public List<string> Isbns { get; set; } = [];
}