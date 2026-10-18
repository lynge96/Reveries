namespace Reveries.Integration.Storage;

public sealed record ObjectUpload(
    string Key,
    byte[] Content,
    string ContentType,
    string CacheControl,
    IReadOnlyDictionary<string, string> Metadata);
