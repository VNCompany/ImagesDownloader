using System.Net.Http;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ImagesDownloader.Services.Download;

internal enum DownloadCollectionStatus
{
    Waiting,
    Process,
    Canceled,
    Done
}

internal class DownloadCollection(IEnumerable<DownloadCollectionItem> items) : INotifyPropertyChanged, IDisposable
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly HttpClient _client = new();

    private CancellationTokenSource? _cts;

    public DownloadCollectionItem[] Items { get; } = items.ToArray();
    public ObservableCollection<DownloadLogItem> Log { get; } = [];

    public bool IsCompleted => Items.Length == Log.Count;

    private float _downloadPercent = 0;
    public float DownloadPercent
    {
        get => _downloadPercent;
        set
        {
            _downloadPercent = value;
            OnPropertyChanged(nameof(DownloadPercent));
        }
    }

    private DownloadCollectionStatus _status;
    public DownloadCollectionStatus Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged(nameof(Status));
        }
    }

    public async Task Download(
        int downloadsPerThreadCount, 
        Action<DownloadCollectionItem, Exception> errorCallback,
        CancellationToken cancellationToken)
    {
        if (_cts != null)
            _cts.Dispose();

        Status = DownloadCollectionStatus.Process;
        using var sema = new SemaphoreSlim(downloadsPerThreadCount);
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var token = _cts.Token;
        var queue = Items.Where(x => x.Status < DownloadCollectionItemStatus.Done);
        var tasks = new List<Task>();
        foreach (var item in queue)
        {
            if (token.IsCancellationRequested)
                break;
            tasks.Add(Task.Run(async () =>
            {
                await sema.WaitAsync(token);
                await item.Download(_client, OnItemDownloaded, errorCallback, token);
                sema.Release();
            }, CancellationToken.None));
        }

        await Task.WhenAll(tasks);
        Status = IsCompleted ? DownloadCollectionStatus.Done : DownloadCollectionStatus.Canceled;
    }

    public void Dispose()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
        _client.Dispose();
    }

    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void OnItemDownloaded(DownloadCollectionItem item)
    {
        lock (this)
        {
            Log.Add(new DownloadLogItem(item.Url, item.Status == DownloadCollectionItemStatus.Done));
            DownloadPercent = (float)Math.Round((double)Log.Count / Items.Length * 100.0, 2);
        }
    }
}
