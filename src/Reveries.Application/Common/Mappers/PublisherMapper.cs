using Reveries.Application.Extensions;
using Reveries.Core.Helpers;
using Reveries.Core.Models;

namespace Reveries.Application.Common.Mappers;

public static class PublisherMapper
{
    public static Publisher ToPublisher(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
            return null!;

        var normalizedName = PublisherNormalizer.NormalizePublisher(rawName);

        return new Publisher
        {
            Name = normalizedName,
        };
    }
}