using System.ComponentModel;
using System.Collections.ObjectModel;

using ImagesDownloader.Models;
using ImagesDownloader.Exceptions;
using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Tasks;

public class CollectionItemsTask : INotifyPropertyChanged, IDisposable
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly IDownloader _downloader = DownloaderFactory.CreateDownloader();
    private readonly SemaphoreSlim _semaResource = new(1);
    private readonly SemaphoreSlim _sema;
    private readonly Queue<DownloadItem> _queue = [];

    public string Name { get; }
    public int ItemsCount { get; }
    public IReadOnlyCollection<DownloadItem> Queue => _queue;
    public ObservableCollection<DownloadItem> CompletedItems { get; }

    public double ProgressPercent
    {
        get
        {
            _semaResource.Wait();
            var result = (double)CompletedItems.Count / ItemsCount * 100.0;
            _semaResource.Release();
            return result;
        }
    }

    public CollectionItemsTask(string name, IEnumerable<DownloadItem> items, int poolSize)
    {
        _sema = new SemaphoreSlim(poolSize);

        Name = name;
        _queue = new(items);
        ItemsCount = _queue.Count;
        if (ItemsCount == 0) throw new ArgumentException($"Collection {nameof(items)} is empty");
        CompletedItems = [];
        CompletedItems.CollectionChanged += (s, e) => PropertyChanged?.Invoke(this, new(nameof(ProgressPercent)));
    }

    public void Dispose()
    {
        _downloader.Dispose();
        _semaResource.Dispose();
        _sema.Dispose();
    }

    /// <summary>
    /// Cancel <paramref name="cancellationToken"/> before disposing object!!!
    /// </summary>
    public async Task Start(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return;

        var tasks = new List<Task>();
        while (_queue.Count != 0)
        {
            tasks.Add(Task.Run(async () =>
            {
                var item = _queue.Dequeue();
                var args = new ItemSavedEventArgs(this, item, false, null);
                try
                {
                    await _sema.WaitAsync();
                    if (cancellationToken.IsCancellationRequested) 
                        throw new SaveDataException(SaveDataExceptionType.TaskCanceled, message: null);
                    await _downloader.SaveData(item.Source, item.OutputPath, cancellationToken);
                    item.IsSuccess = true;
                    await OnItemSaved(args with { IsSuccess = true });
                }
                catch (SaveDataException sdex)
                {
                    if (sdex.Type == SaveDataExceptionType.TaskCanceled)
                        await ReturnItemToQueue(item);
                    else
                        await OnItemSaved(args with { Exception = sdex, IsSuccess = false });
                }
                finally
                {
                    _sema.Release();
                }
            }, CancellationToken.None));
        }
        await Task.WhenAll(tasks);
    }

    private async Task OnItemSaved(ItemSavedEventArgs args)
    {
        try
        {
            await _semaResource.WaitAsync();
            CompletedItems.Insert(0, args.Item);
        }
        finally
        {
            _semaResource.Release();
        }
    }

    private async Task ReturnItemToQueue(DownloadItem item)
    {
        try
        {
            await _semaResource.WaitAsync();
            _queue.Enqueue(item);
        }
        finally
        {
            _semaResource.Release();
        }
    }
}
