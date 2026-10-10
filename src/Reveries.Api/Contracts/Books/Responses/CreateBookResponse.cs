using System.ComponentModel;

namespace Reveries.Api.Contracts.Books.Responses;

public sealed record CreateBookResponse([property: Description("Identifier of the created edition.")] Guid Id);