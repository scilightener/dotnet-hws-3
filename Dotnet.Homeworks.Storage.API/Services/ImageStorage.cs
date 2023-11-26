using System.Reactive.Linq;
using Dotnet.Homeworks.Shared.Dto;
using Dotnet.Homeworks.Storage.API.Dto.Internal;
using Minio;
using Minio.DataModel;
using Minio.Exceptions;

namespace Dotnet.Homeworks.Storage.API.Services;

public class ImageStorage : IStorage<Image>
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;
    
    public ImageStorage(IMinioClient minioClient, string bucketName)
    {
        _minioClient = minioClient;
        _bucketName = bucketName;
    }
    
    public async Task<Result> PutItemAsync(Image item, CancellationToken cancellationToken = default)
    {
        if (item.Content is null) throw new ArgumentNullException(nameof(item.Content));
        if (await ExistsAsync(item.FileName, cancellationToken))
            return new Result(false, "An item with this name already exists");
        item.Content.Position = 0;
        // first, place an image in pending bucket, then our background service will move it to the destination
        item.Metadata[Constants.MetadataKeys.Destination] = _bucketName;
        var putObjArgs = new PutObjectArgs()
            .WithBucket(Constants.Buckets.Pending)
            .WithObject(item.FileName)
            .WithStreamData(item.Content)
            .WithContentType(item.ContentType)
            .WithObjectSize(item.Content.Length)
            .WithHeaders(item.Metadata);
        try
        {
            await _minioClient.PutObjectAsync(putObjArgs, cancellationToken);
        }
        catch (Exception e)
        {
            return new Result(false, e.Message);
        }
    
        return new Result(true);
    }
    
    public async Task<Image?> GetItemAsync(string itemName, CancellationToken cancellationToken = default)
    {
        var outputStream = new MemoryStream();
        var getObjectArgs = new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(itemName)
            .WithCallbackStream(s => s.CopyTo(outputStream));
        ObjectStat objectStat;
        try
        {
            objectStat = await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);
        }
        catch (ObjectNotFoundException)
        {
            return null;
        }
    
        outputStream.Position = 0;
        return new Image(outputStream, objectStat.ObjectName, objectStat.ContentType, objectStat.MetaData);
    }
    
    public async Task<Result> RemoveItemAsync(string itemName, CancellationToken cancellationToken = default)
    {
        var removeObjArgs = new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(itemName);
        try
        {
            await _minioClient.RemoveObjectAsync(removeObjArgs, cancellationToken);
        }
        catch (Exception e)
        {
            return new Result(false, e.Message);
        }
    
        return new Result(true);
    }
    
    public async Task<IEnumerable<string>> EnumerateItemNamesAsync(CancellationToken cancellationToken = default)
    {
        var listObjArgs = new ListObjectsArgs()
            .WithBucket(_bucketName);
        return await _minioClient.ListObjectsAsync(listObjArgs, cancellationToken).Select(t => t.Key).ToList();
    }

    public async Task<Result> CopyItemToBucketAsync(string itemName, string destinationBucketName,
        CancellationToken cancellationToken = default)
    {
        var copySrcObjArgs = new CopySourceObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(itemName);
        var copyObjArgs = new CopyObjectArgs()
            .WithBucket(destinationBucketName)
            .WithObject(itemName)
            .WithCopyObjectSource(copySrcObjArgs);

        try
        {
            await _minioClient.CopyObjectAsync(copyObjArgs, cancellationToken);
        }
        catch (Exception e)
        {
            return new Result(false, e.Message);
        }

        return new Result(true);
    }

    private async Task<bool> ExistsAsync(string itemName, CancellationToken cancellationToken = default)
    {
        try
        {
            var objStat = await _minioClient.GetObjectAsync(
                new GetObjectArgs().WithBucket(_bucketName).WithObject(itemName).WithCallbackStream(_ => { }),
                cancellationToken);
            return objStat is not null;
        }
        catch (ObjectNotFoundException)
        {
            return false;
        }
    }
}