namespace ProjectTracker.Application.Extensions;

internal static class ObjectStorageExtensions
{
    public static async Task<string> UploadIssueAttachment(
        this IObjectStorage storage,
        Guid projectId,
        Guid issueId,
        string name,
        Stream stream,
        string mimeType,
        CancellationToken ct)
    {
        var storageKey = $"projects/{projectId}/issues/{issueId}/{Guid.NewGuid().ToString().Substring(0, 8)}-{name}";

        var uploaded = await storage.UploadAsync(storageKey, stream, mimeType, ct);

        if (!uploaded)
        {
            throw new FailedToUploadFileException(storageKey);
        }

        return storageKey;
    }

    public static async Task<string> UploadProjectAttachment(
        this IObjectStorage storage,
        Guid projectId,
        string name,
        Stream stream,
        string mimeType,
        CancellationToken ct)
    {
        var storageKey = $"projects/{projectId}/{Guid.NewGuid().ToString().Substring(0, 8)}-{name}";

        var uploaded = await storage.UploadAsync(storageKey, stream, mimeType, ct);

        if (!uploaded)
        {
            throw new FailedToUploadFileException(storageKey);
        }

        return storageKey;
    }
}
