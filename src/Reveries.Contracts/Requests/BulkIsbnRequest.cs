namespace Reveries.Contracts.Requests;

public class BulkIsbnRequest
{
    public List<string> Isbns { get; set; } = new();
}