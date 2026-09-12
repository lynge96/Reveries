using System.Text.Json.Serialization;

namespace Reveries.Integration.Isbndb.Dtos;

public sealed record IsbndbBulkIsbnRequest(
    [property: JsonPropertyName("isbns")] IReadOnlyList<string> Isbns);