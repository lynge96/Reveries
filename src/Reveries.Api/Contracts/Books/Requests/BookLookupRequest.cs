using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Reveries.Api.Contracts.Books.Requests;

public sealed record BookLookupRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    [Description("ISBNs to look up (ISBN-10 or ISBN-13). Between 1 and 100 entries.")]
    public List<string> Isbns { get; init; } = [];
}
