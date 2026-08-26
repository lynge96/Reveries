using Reveries.Domain.Helpers;

namespace Reveries.Domain.Publishers;

public class Publisher
{
    public PublisherId Id { get; private init; }
    public string Name { get; }
    public string NormalizedName => Name.ToLowerInvariant();

    private Publisher(PublisherId id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString() => Name;

    public static Publisher? TryCreate(string? name)
    {
        var normalizedName = PublisherNameNormalizer.Normalize(name);
        if (string.IsNullOrWhiteSpace(normalizedName))
            return null;

        return new Publisher(PublisherId.New(), normalizedName);
    }

    public static Publisher Reconstitute(PublisherId id, string name)
    {
        return new Publisher(id, name);
    }
}