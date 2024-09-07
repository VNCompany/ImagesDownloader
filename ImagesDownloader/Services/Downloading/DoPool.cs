using System.Diagnostics.CodeAnalysis;

using ImagesDownloader.Interfaces;
using ImagesDownloader.Models;

namespace ImagesDownloader.Services.Downloading;

internal class DoPool : IDisposable
{
    public event EventHandler<DoInfo>? ItemDownloaded;

    private readonly int _sleepTime;
    private readonly List<DownloadItemCollection> _collections;
    private readonly IDownloadClient _downloadClient;
    private readonly CancellationToken _mainCancellationToken;
    private readonly SemaphoreSlim _resourcesSemaphore = new(1);
    private readonly Dictionary<DownloadItemCollection, CancellationTokenSource> _cancellationSources;

    private Task[]? _tasks;

    public DoPool(
        int poolSize,
        int sleepTime,
        List<DownloadItemCollection> collections, 
        IDownloadClient downloadClient, 
        CancellationToken cancellationToken)
    {
        _sleepTime = sleepTime;
        _collections = collections;
        _downloadClient = downloadClient;
        _mainCancellationToken = cancellationToken;

        _cancellationSources = new(_collections.Count);
        foreach (var collection in collections)
            _cancellationSources.Add(collection, CancellationTokenSource.CreateLinkedTokenSource(cancellationToken));

        _tasks = new Task[poolSize];
        for (int i = 0; i < _tasks.Length; ++i)
            _tasks[i] = Task.Run(Do, CancellationToken.None);
    }

    public void Wait()
    {
        if (_tasks != null)
        {
            Task.WaitAll(_tasks);
            _tasks = null;
        }
    }

    public void Dispose()
    {
        foreach (var cancellationSource in _cancellationSources.Values)
        {
            cancellationSource.Cancel();
            cancellationSource.Dispose();
        }
        Wait();
        _cancellationSources.Clear();
    }

    private bool TakeItem([NotNullWhen(true)] out DoInfo? info)
    {
        info = null;
        try
        {
            _resourcesSemaphore.Wait();
            if (_collections.Count == 0) return false;
            var selectedCollection = _collections[^1];
            info = new DoInfo(selectedCollection, selectedCollection.ItemsQueue.Dequeue());
            if (selectedCollection.ItemsQueue.Count == 0)
                _collections.RemoveAt(_collections.Count - 1);
            return true;
        }
        finally
        {
            _resourcesSemaphore.Release();
        }
    }

    private async Task Do()
    {
        while (TakeItem(out DoInfo? info))
        {
            if (_mainCancellationToken.IsCancellationRequested) return;
            var cancellationToken = _cancellationSources[info.Collection].Token;
            try
            {
                await _downloadClient.SaveData(info.Item.Source, info.Item.OutputPath, cancellationToken);
                info.Item.IsSuccess = true;
                await Task.Delay(_sleepTime, _mainCancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                if (!info.Item.IsSuccess)
                    info.Exception = ex;
            }
            info.Item.IsSuccess = info.IsSuccess;
            ItemDownloaded?.Invoke(this, info);
        }
    }
}
