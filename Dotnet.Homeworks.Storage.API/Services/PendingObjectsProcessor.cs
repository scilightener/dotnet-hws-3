using Dotnet.Homeworks.Storage.API.Dto.Internal;

namespace Dotnet.Homeworks.Storage.API.Services;

public class PendingObjectsProcessor : BackgroundService
{
    private readonly IStorageFactory _storageFactory;
    private IStorage<Image> _storage = null!;

    public PendingObjectsProcessor(IStorageFactory storageFactory)
    {
        _storageFactory = storageFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _storage = await _storageFactory.CreateImageStorageWithinBucketAsync(Constants.Buckets.Pending);

        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessPendingObjects(stoppingToken);
            await Task.Delay(Constants.PendingObjectProcessor.Period, stoppingToken);
        }
    }

    private async Task ProcessPendingObjects(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return;
        try
        {
            var items = await _storage.EnumerateItemNamesAsync(cancellationToken);

            foreach (var item in items)
            {
                var obj = await _storage.GetItemAsync(item, cancellationToken);
                // if haven't found any destination, just delete as garbage
                if (obj!.Metadata.TryGetValue(Constants.MetadataKeys.Destination, out var destinationBucket))
                    await _storage.CopyItemToBucketAsync(item, destinationBucket, cancellationToken);
                await _storage.RemoveItemAsync(item, cancellationToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}