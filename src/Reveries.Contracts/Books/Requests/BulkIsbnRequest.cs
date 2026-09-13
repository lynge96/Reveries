using System.ComponentModel.DataAnnotations;

namespace Reveries.Contracts.Books.Requests;

public class BulkIsbnRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public List<string> Isbns { get; set; } = [];
}
