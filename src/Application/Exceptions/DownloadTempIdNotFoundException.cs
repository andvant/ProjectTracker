namespace ProjectTracker.Application.Exceptions;

public class DownloadTempIdNotFoundException : ApplicationException
{
    public Guid TempId { get; }

    public DownloadTempIdNotFoundException(Guid tempId)
        : base($"Download with temp id '{tempId}' was not found.")
    {
        TempId = tempId;
    }
}
