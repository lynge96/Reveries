namespace Reveries.Contracts.Books.Requests;

public class GetAllBooksRequest
{
    /// <summary>
    /// If true, only returns read books. If false, only unread. If null, returns all.
    /// </summary>
    public bool? IsRead { get; set; }
}