using Reveries.Application.Extensions;
using Reveries.Core.Models;

namespace Reveries.Application.Common.Mappers;

public static class AuthorMapper
{
    public static Author ToAuthor(string rawName)
    {
        var (firstName, lastName, normalizedName) = AuthorNameNormalizer.NormalizeAuthorName(rawName);

        var author = new Author
        {
            NormalizedName = normalizedName,
            FirstName = string.IsNullOrWhiteSpace(firstName) ? null : firstName,
            LastName = string.IsNullOrWhiteSpace(lastName) ? null : lastName,
        };

        return author;
    }
}