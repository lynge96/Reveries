namespace Reveries.Application.Books.Models;

public sealed record SourcedBooks(BookSource Source, IReadOnlyList<BookCandidate> Books);