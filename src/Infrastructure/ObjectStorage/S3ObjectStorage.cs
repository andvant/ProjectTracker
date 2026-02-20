using System.Net;
using System.Net.Mime;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using ProjectTracker.Application.Interfaces;

namespace ProjectTracker.Infrastructure.ObjectStorage;

public class S3ObjectStorage : IObjectStorage
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucket;

    public S3ObjectStorage(IOptions<S3StorageOptions> storageOptions)
    {
        var options = storageOptions.Value;
        _bucket = options.Bucket;

        var config = new AmazonS3Config
        {
            ServiceURL = options.Endpoint,
            ForcePathStyle = true,
            UseHttp = true
        };

        _s3Client = new AmazonS3Client(
            new BasicAWSCredentials(options.AccessKey, options.SecretKey),
            config);
    }

    public async Task<Stream> GetAsync(string storageKey, CancellationToken ct)
    {
        var request = new GetObjectRequest
        {
            BucketName = _bucket,
            Key = storageKey
        };

        var response = await _s3Client.GetObjectAsync(request, ct);

        return response.ResponseStream;
    }

    public async Task<bool> UploadAsync(string storageKey, Stream stream, string mimeType, CancellationToken ct)
    {
        await EnsureBucketCreated(ct);

        var request = new PutObjectRequest
        {
            BucketName = _bucket,
            Key = storageKey,
            InputStream = stream,
            ContentType = mimeType ?? MediaTypeNames.Application.Octet
        };

        var response = await _s3Client.PutObjectAsync(request, ct);

        return response.HttpStatusCode is HttpStatusCode.NoContent or HttpStatusCode.OK;
    }

    public async Task<bool> DeleteAsync(string storageKey, CancellationToken ct)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _bucket,
            Key = storageKey
        };

        var response = await _s3Client.DeleteObjectAsync(request, ct);

        return response.HttpStatusCode is HttpStatusCode.NoContent or HttpStatusCode.OK;
    }

    public async Task<IEnumerable<string>> ListAsync(CancellationToken ct)
    {
        await EnsureBucketCreated(ct);

        var request = new ListObjectsV2Request
        {
            BucketName = _bucket
        };

        var response = await _s3Client.ListObjectsV2Async(request, ct);

        return response.S3Objects?.Select(o => o.Key) ?? Enumerable.Empty<string>();
    }

    private async Task EnsureBucketCreated(CancellationToken ct)
    {
        if (await BucketExistsAsync(ct)) return;

        var request = new PutBucketRequest
        {
            BucketName = _bucket,
            UseClientRegion = false
        };

        await _s3Client.PutBucketAsync(request, ct);
    }

    private async Task<bool> BucketExistsAsync(CancellationToken ct)
    {
        var response = await _s3Client.ListBucketsAsync(ct);

        return response.Buckets?.Any(b => b.BucketName == _bucket) ?? false;
    }
}
