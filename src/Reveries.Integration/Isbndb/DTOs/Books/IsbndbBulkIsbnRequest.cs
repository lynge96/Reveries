using System.Text.Json.Serialization;

namespace Reveries.Integration.Isbndb.DTOs.Books;

public sealed record IsbndbBulkIsbnRequest(
    [property: JsonPropertyName("isbns")] IReadOnlyList<string> Isbns);