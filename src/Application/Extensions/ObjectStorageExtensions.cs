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
        var storageKey = $"projects/{projectId}/issues/{issueId}/{GetNameWithRandomSuffix(name)}";

        await UploadFile(storage, stream, mimeType, storageKey, ct);

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
        var storageKey = $"projects/{projectId}/{GetNameWithRandomSuffix(name)}";

        await UploadFile(storage, stream, mimeType, storageKey, ct);

        return storageKey;
    }

    private static async Task UploadFile(IObjectStorage storage,
        Stream stream, string mimeType, string storageKey, CancellationToken ct)
    {
        var uploaded = await storage.UploadAsync(storageKey, stream, mimeType, ct);

        if (!uploaded)
        {
            throw new FailedToUploadFileException(storageKey);
        }
    }

    private static string GetNameWithRandomSuffix(string name) =>
        $"{Path.GetFileNameWithoutExtension(name)}-{GetRandomString()}{Path.GetExtension(name)}";

    private static string GetRandomString() => Guid.NewGuid().ToString().Substring(0, 8);
}
