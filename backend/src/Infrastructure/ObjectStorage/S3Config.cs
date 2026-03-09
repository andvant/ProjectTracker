namespace ProjectTracker.Infrastructure.ObjectStorage;

internal class S3Config
{
    public required string Endpoint { get; set; }
    public required string AccessKey { get; set; }
    public required string SecretKey { get; set; }
    public required string Bucket { get; set; }
}
