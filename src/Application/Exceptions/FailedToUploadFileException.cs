namespace ProjectTracker.Application.Exceptions;

public class FailedToUploadFileException : ApplicationException
{
    public string StorageKey { get; }

    public FailedToUploadFileException(string storageKey)
        : base($"Failed to upload file with storage key '{storageKey}'.")
    {
        StorageKey = storageKey;
    }
}
