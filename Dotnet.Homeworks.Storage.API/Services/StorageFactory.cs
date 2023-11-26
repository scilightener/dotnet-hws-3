using Dotnet.Homeworks.Storage.API.Dto.Internal;
using Minio;

namespace Dotnet.Homeworks.Storage.API.Services;

public class StorageFactory : IStorageFactory
{
    private readonly MinioClient _minioClient;

    public StorageFactory(MinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    public async Task<IStorage<Image>> CreateImageStorageWithinBucketAsync(string bucketName)
    {
        if (!await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName)))
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
        return new ImageStorage(_minioClient, bucketName);
    }
}