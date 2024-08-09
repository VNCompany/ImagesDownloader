using ImagesDownloader.Interfaces;
using ImagesDownloader.Models;

namespace ImagesDownloader.Common;

internal class CollectionsDownloader : IDisposable
{
    private enum DownloadItemStatus
    {
        Canceled = 1, Failed = 2, Success = 3
    }

    private readonly int _downloadItemsPoolSize;
    private readonly SemaphoreSlim _mainSemaphore;
    private readonly DownloadClient _downloadClient;

    public CollectionsDownloader(int downloadCollectionsPoolSize, int downloadItemsPoolSize)
    {
        _downloadItemsPoolSize = downloadItemsPoolSize;

        _mainSemaphore = new SemaphoreSlim(downloadCollectionsPoolSize);
        _downloadClient = new DownloadClient();
    }

    public async Task Start(IEnumerable<DownloadCollection> collections, CancellationToken cancellationToken)
    {
        var tasksPool = collections
            .Select(x => QueueDownloadCollection(x, cancellationToken))
            .ToArray();
        await Task.WhenAll(tasksPool);
    }

    public void Dispose()
    {
        _downloadClient.Dispose();
        _mainSemaphore.Dispose();
    }

    private async Task QueueDownloadCollection(DownloadCollection collection, CancellationToken cancellationToken)
    {
        if (collection.Queue.Count == 0)
            return;

        try
        {
            await _mainSemaphore.WaitAsync(cancellationToken);

            object resourceLocker = new();
            var collectionSemaphore = new SemaphoreSlim(_downloadItemsPoolSize);
            var tasks = new List<Task>();
            while (collection.Queue.Count > 0)
            {
                DownloadItem item;
                lock (collection.Queue)
                    item = collection.Queue.Dequeue();

                tasks.Add(QueueDownloadItem(
                    item,
                    collectionSemaphore,
                    cancellationToken,
                    callback: (item, status) =>
                    {
                        if (status > DownloadItemStatus.Canceled)
                            collection.OnItemDownloaded(item, status == DownloadItemStatus.Success);
                        else
                        {
                            lock (collection.Queue)              // Задача отменена, но файл не скачался,
                                collection.Queue.Enqueue(item);  // поэтому возвращаем обратно в очередь загрузки
                        }
                    }));
            }

            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException) { }
        finally
        {
            _mainSemaphore.Release();
        }
    }

    private async Task QueueDownloadItem(
        DownloadItem item,
        SemaphoreSlim semaphore,
        CancellationToken cancellationToken,
        Action<DownloadItem, DownloadItemStatus> callback)
    {
        try
        {
            await semaphore.WaitAsync(cancellationToken);
            await _downloadClient.DownloadFile(item.Url, item.OutputPath, cancellationToken);
            callback.Invoke(item, DownloadItemStatus.Success);
        }
        catch (OperationCanceledException)
        {
            callback.Invoke(item, DownloadItemStatus.Canceled);
        }
        catch (Exception)
        {
            callback.Invoke(item, DownloadItemStatus.Failed);
        }
        finally
        {
            semaphore.Release();
        }
    }
}
