using System.ComponentModel;

namespace Reveries.Api.Contracts.Books.Responses;

public sealed record BookExistsResponse([property: Description("True when a book with the given ISBN exists in the catalog.")] bool Exists);
