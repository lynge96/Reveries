namespace Reveries.Integration.Storage;

public interface IObjectStore
{
    Task<bool> ExistsAsync(string key, CancellationToken ct);

    Task PutAsync(ObjectUpload upload, CancellationToken ct);

    string PublicUrl(string key);
}
