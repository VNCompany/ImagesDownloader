using System.Collections.ObjectModel;
using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Services.Download;

internal class DownloadService : IDisposable
{
    private readonly ILogger _logger;
    private readonly AppSettings _appSettings;

    private CancellationTokenSource? _cts;
    private Task? _currentTask;

    public ObservableCollection<DownloadCollection> DownloadCollections { get; } = [];
    public Action<IList<DownloadCollection>>? DownloadCompletedCallback { get; set; }

    public DownloadService(ILogger logger, AppSettings appSettings)
    {
        _logger = logger;
        _appSettings = appSettings;
    }

    public void AddCollection(DownloadCollection item)
    {
        DownloadCollections.Add(item);
    }

    public void RemoveCollection(DownloadCollection item)
    {
        item.Dispose();
        DownloadCollections.Remove(item);
    }

    public void Start()
    {
        _cts = new CancellationTokenSource();
        _currentTask = Start(_cts.Token);
    }

    public void Stop()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _currentTask?.Wait();
            _cts.Dispose();
            _cts = null;
        }
    }

    public void Dispose()
    {
        Stop();
    }

    private async Task Start(CancellationToken cancellationToken)
    {
        using var sema = new SemaphoreSlim(_appSettings.MaxDownloadingCollectionsCount);
        foreach (var collection in DownloadCollections.Where(x => !x.IsCompleted))
        {
            await sema.WaitAsync(cancellationToken);
            await collection.Download(_appSettings.MaxDownloadingItemsCount, OnItemDownloadFailed, cancellationToken);
            sema.Release();
        }
        DownloadCompletedCallback?.Invoke(DownloadCollections);
    }

    private void OnItemDownloadFailed(DownloadCollectionItem item, Exception exception)
    {
        _logger.LogError($"{exception.GetType().Name}: " +
            $"{exception.Message} \n\tUrl: {item.Url} \n\tPath: {item.SavePath}", "Load item");
    }
}
